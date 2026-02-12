using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMSMini.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUnifiedPaymentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckInId",
                table: "Vouchers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BanquetBookingId",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDue",
                table: "Invoices",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaid",
                table: "Invoices",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    CheckInId = table.Column<int>(type: "int", nullable: true),
                    BanquetBookingId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VoucherId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_BanquetBookings_BanquetBookingId",
                        column: x => x.BanquetBookingId,
                        principalTable: "BanquetBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_CheckIn_CheckInId",
                        column: x => x.CheckInId,
                        principalTable: "CheckIn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 872, DateTimeKind.Utc).AddTicks(7793));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 872, DateTimeKind.Utc).AddTicks(7797));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 872, DateTimeKind.Utc).AddTicks(7801));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 872, DateTimeKind.Utc).AddTicks(7804));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 872, DateTimeKind.Utc).AddTicks(7807));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 17, 12, 6, 876, DateTimeKind.Utc).AddTicks(6869));

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_BanquetBookingId",
                table: "Vouchers",
                column: "BanquetBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_PaymentId",
                table: "Vouchers",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BanquetBookingId",
                table: "Payments",
                column: "BanquetBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CheckInId",
                table: "Payments",
                column: "CheckInId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CompanyId",
                table: "Payments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReceiptNumber",
                table: "Payments",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SourceType",
                table: "Payments",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VoucherId",
                table: "Payments",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_BanquetBookings_BanquetBookingId",
                table: "Vouchers",
                column: "BanquetBookingId",
                principalTable: "BanquetBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Payments_PaymentId",
                table: "Vouchers",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_BanquetBookings_BanquetBookingId",
                table: "Vouchers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Payments_PaymentId",
                table: "Vouchers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_BanquetBookingId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_PaymentId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "BanquetBookingId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "BalanceDue",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalPaid",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "CheckInId",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 626, DateTimeKind.Utc).AddTicks(9090));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 626, DateTimeKind.Utc).AddTicks(9096));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 626, DateTimeKind.Utc).AddTicks(9099));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 626, DateTimeKind.Utc).AddTicks(9103));

            migrationBuilder.UpdateData(
                table: "MGuestTypes",
                keyColumn: "GuestTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 626, DateTimeKind.Utc).AddTicks(9106));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 12, 12, 30, 57, 641, DateTimeKind.Utc).AddTicks(4384));
        }
    }
}
