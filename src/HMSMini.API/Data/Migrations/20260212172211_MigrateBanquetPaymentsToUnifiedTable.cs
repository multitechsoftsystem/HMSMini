using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMSMini.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateBanquetPaymentsToUnifiedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Copy existing BanquetPayment records into the unified Payments table
            //    - SourceType = 1 (Banquet)
            //    - PaymentType/PaymentMode enum values are identical (0-3 / 0-4)
            //    - CompanyId resolved from BanquetBookings
            //    - Preserve receipt numbers, audit fields, and soft deletes
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [Payments] OFF;

                INSERT INTO [Payments] (
                    [ReceiptNumber],
                    [SourceType],
                    [CheckInId],
                    [BanquetBookingId],
                    [CompanyId],
                    [PaymentDate],
                    [PaymentType],
                    [PaymentMode],
                    [Amount],
                    [ReferenceNumber],
                    [ReceivedBy],
                    [Remarks],
                    [VoucherId],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy],
                    [DeletedAt],
                    [DeletedBy]
                )
                SELECT
                    bp.[ReceiptNumber],
                    1,                          -- SourceType = Banquet
                    NULL,                       -- CheckInId (not applicable)
                    bp.[BanquetBookingId],
                    bb.[CompanyId],             -- Denormalized from BanquetBooking
                    bp.[PaymentDate],
                    bp.[PaymentType],           -- Enum values match 1:1
                    bp.[PaymentMode],           -- Enum values match 1:1
                    bp.[Amount],
                    bp.[ReferenceNumber],
                    bp.[ReceivedBy],
                    N'Migrated from BanquetPayments',  -- Remarks
                    NULL,                       -- VoucherId (no voucher for legacy payments)
                    bp.[CreatedAt],
                    bp.[CreatedBy],
                    bp.[UpdatedAt],
                    bp.[UpdatedBy],
                    bp.[DeletedAt],
                    bp.[DeletedBy]
                FROM [BanquetPayments] bp
                INNER JOIN [BanquetBookings] bb ON bp.[BanquetBookingId] = bb.[Id];
            ");

            // 2. Set Invoice.BalanceDue = GrandTotal for all existing invoices
            //    (since no room payments exist in the unified table yet)
            migrationBuilder.Sql(@"
                UPDATE [Invoices]
                SET [BalanceDue] = [GrandTotal],
                    [TotalPaid] = 0
                WHERE [DeletedAt] IS NULL;
            ");

            // 3. Update BanquetInvoice TotalPaid/BalanceDue from migrated payments
            //    Recalculate from the unified Payments table
            migrationBuilder.Sql(@"
                UPDATE bi
                SET bi.[TotalPaid] = ISNULL(pmt.TotalPaid, 0),
                    bi.[BalanceDue] = bi.[GrandTotal] - ISNULL(pmt.TotalPaid, 0),
                    bi.[PaymentStatus] = CASE
                        WHEN ISNULL(pmt.TotalPaid, 0) >= bi.[GrandTotal] THEN 'Paid'
                        WHEN ISNULL(pmt.TotalPaid, 0) > 0 THEN 'PartiallyPaid'
                        ELSE 'Unpaid'
                    END
                FROM [BanquetInvoices] bi
                LEFT JOIN (
                    SELECT
                        [BanquetBookingId],
                        SUM(CASE WHEN [PaymentType] = 3 THEN -[Amount] ELSE [Amount] END) AS TotalPaid
                    FROM [Payments]
                    WHERE [DeletedAt] IS NULL
                      AND [SourceType] = 1
                    GROUP BY [BanquetBookingId]
                ) pmt ON bi.[BanquetBookingId] = pmt.[BanquetBookingId]
                WHERE bi.[DeletedAt] IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove migrated records (identifiable by the Remarks field)
            migrationBuilder.Sql(@"
                DELETE FROM [Payments]
                WHERE [SourceType] = 1
                  AND [Remarks] = N'Migrated from BanquetPayments';
            ");

            // Reset Invoice TotalPaid/BalanceDue
            migrationBuilder.Sql(@"
                UPDATE [Invoices]
                SET [TotalPaid] = 0,
                    [BalanceDue] = 0
                WHERE [DeletedAt] IS NULL;
            ");

            // Reset BanquetInvoice to original state
            // (BanquetInvoice already had TotalPaid/BalanceDue, so restore from BanquetPayments)
            migrationBuilder.Sql(@"
                UPDATE bi
                SET bi.[TotalPaid] = ISNULL(pmt.TotalPaid, 0),
                    bi.[BalanceDue] = bi.[GrandTotal] - ISNULL(pmt.TotalPaid, 0),
                    bi.[PaymentStatus] = CASE
                        WHEN ISNULL(pmt.TotalPaid, 0) >= bi.[GrandTotal] THEN 'Paid'
                        WHEN ISNULL(pmt.TotalPaid, 0) > 0 THEN 'PartiallyPaid'
                        ELSE 'Unpaid'
                    END
                FROM [BanquetInvoices] bi
                LEFT JOIN (
                    SELECT
                        [BanquetBookingId],
                        SUM(CASE WHEN [PaymentType] = 3 THEN -[Amount] ELSE [Amount] END) AS TotalPaid
                    FROM [BanquetPayments]
                    WHERE [DeletedAt] IS NULL
                    GROUP BY [BanquetBookingId]
                ) pmt ON bi.[BanquetBookingId] = pmt.[BanquetBookingId]
                WHERE bi.[DeletedAt] IS NULL;
            ");
        }
    }
}
