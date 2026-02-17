namespace HMSMini.Web.Models;

// Enums
public enum BanquetBookingStatus
{
    Enquiry = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

public enum BanquetPricingType
{
    PerPlate = 0,
    Package = 1
}

public enum TaxType
{
    CgstSgst = 0,
    Igst = 1
}

// Banquet Hall Models
public class BanquetHallModel
{
    public int Id { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
    public decimal RentPerEvent { get; set; }
    public string? Location { get; set; }
    public string? Features { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBanquetHallModel
{
    public string HallName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
    public decimal RentPerEvent { get; set; }
    public string? Location { get; set; }
    public string? Features { get; set; }
    public string? ImagePath { get; set; }
}

public class UpdateBanquetHallModel
{
    public string HallName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
    public decimal RentPerEvent { get; set; }
    public string? Location { get; set; }
    public string? Features { get; set; }
    public string? ImagePath { get; set; }
}

// Event Type Models
public class EventTypeModel
{
    public int Id { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateEventTypeModel
{
    public string EventTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateEventTypeModel
{
    public string EventTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Banquet Service Models
public class BanquetServiceModel
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal DefaultRate { get; set; }
    public string? Unit { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBanquetServiceModel
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal DefaultRate { get; set; }
    public string? Unit { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

public class UpdateBanquetServiceModel
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal DefaultRate { get; set; }
    public string? Unit { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

// Banquet Booking Models
public class BanquetBookingListModel
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public BanquetBookingStatus Status { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public decimal HallRent { get; set; }
}

public class BanquetBookingDetailModel
{
    public int Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public int BanquetHallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int EventTypeId { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }
    public BanquetBookingStatus Status { get; set; }
    public BanquetPricingType PricingType { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public int? CheckInId { get; set; }
    public TaxType TaxType { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal HallRent { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<BanquetBookingMenuModel> Menus { get; set; } = new();
    public List<BanquetBookingServiceModel> Services { get; set; } = new();
    public List<BanquetChargeModel> Charges { get; set; } = new();
    public List<BanquetPaymentModel> Payments { get; set; } = new();
}

public class CreateBanquetBookingModel
{
    public int BanquetHallId { get; set; }
    public int EventTypeId { get; set; }
    public DateTime EventDate { get; set; } = DateTime.Today.AddDays(1);
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public BanquetPricingType PricingType { get; set; } = BanquetPricingType.PerPlate;
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public int? CheckInId { get; set; }
    public TaxType TaxType { get; set; } = TaxType.CgstSgst;
    public decimal DiscountPercentage { get; set; }
    public decimal HallRent { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateBanquetBookingModel
{
    public int BanquetHallId { get; set; }
    public int EventTypeId { get; set; }
    public DateTime EventDate { get; set; }
    public TimeSpan EventStartTime { get; set; }
    public TimeSpan EventEndTime { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }
    public BanquetPricingType PricingType { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public int? CheckInId { get; set; }
    public TaxType TaxType { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal HallRent { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateBanquetBookingStatusModel
{
    public BanquetBookingStatus NewStatus { get; set; }
}

// Booking Menu Models
public class BanquetBookingMenuModel
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public int? MenuPackageId { get; set; }
    public string? PackageName { get; set; }
    public int? MenuItemId { get; set; }
    public string? ItemName { get; set; }
    public DateTime MenuDate { get; set; }
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBanquetBookingMenuModel
{
    public int? MenuPackageId { get; set; }
    public int? MenuItemId { get; set; }
    public string? ItemName { get; set; }
    public DateTime MenuDate { get; set; } = DateTime.Today;
    public int Quantity { get; set; } = 1;
    public decimal RatePerPlate { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

public class UpdateBanquetBookingMenuModel
{
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
}

// Booking Service Models
public class BanquetBookingServiceModel
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public int? BanquetServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBanquetBookingServiceModel
{
    public int? BanquetServiceId { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Today;
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Rate { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

public class UpdateBanquetBookingServiceModel
{
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

// Banquet Charge Models
public class BanquetChargeModel
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public DateTime ChargeDate { get; set; }
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBanquetChargeModel
{
    public DateTime ChargeDate { get; set; } = DateTime.Today;
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; } = 1;
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

public class UpdateBanquetChargeModel
{
    public DateTime ChargeDate { get; set; }
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public bool ApplyTax { get; set; } = true;
    public int? VoucherTaxConfigId { get; set; }
}

// Banquet Billing Models
public class BanquetBillPreviewModel
{
    public int BanquetBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }
    public decimal HallRent { get; set; }
    public List<BanquetMenuChargeModel> MenuCharges { get; set; } = new();
    public decimal MenuChargesSubtotal { get; set; }
    public List<BanquetServiceChargeModel> ServiceCharges { get; set; } = new();
    public decimal ServiceChargesSubtotal { get; set; }
    public List<BanquetAdditionalChargeModel> AdditionalCharges { get; set; } = new();
    public decimal AdditionalChargesSubtotal { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalAfterDiscount { get; set; }
    public List<BanquetTaxSummaryModel> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class BanquetInvoiceModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int BanquetBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public int ExpectedGuests { get; set; }
    public int? ActualGuests { get; set; }
    public decimal HallRent { get; set; }
    public List<BanquetMenuChargeModel> MenuCharges { get; set; } = new();
    public decimal MenuChargesSubtotal { get; set; }
    public List<BanquetServiceChargeModel> ServiceCharges { get; set; } = new();
    public decimal ServiceChargesSubtotal { get; set; }
    public List<BanquetAdditionalChargeModel> AdditionalCharges { get; set; } = new();
    public decimal AdditionalChargesSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubtotalBeforeTax { get; set; }
    public List<BanquetTaxSummaryModel> TaxBreakdown { get; set; } = new();
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
    public List<BanquetPaymentHistoryModel> PaymentHistory { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class FinalizeBanquetInvoiceModel
{
    public int? ActualGuests { get; set; }
}

public class BanquetMenuChargeModel
{
    public string? PackageName { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal RatePerPlate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public string? TaxConfigName { get; set; }
    public string? SACCode { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}

public class BanquetServiceChargeModel
{
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public string? TaxConfigName { get; set; }
    public string? SACCode { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}

public class BanquetAdditionalChargeModel
{
    public string ChargeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ApplyTax { get; set; }
    public int? VoucherTaxConfigId { get; set; }
    public string? TaxConfigName { get; set; }
    public string? SACCode { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}

public class BanquetTaxSummaryModel
{
    public string TaxType { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public decimal TotalTaxAmount { get; set; }
}

public class BanquetPaymentHistoryModel
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}

// Banquet Invoice List Model (lightweight for bill search)
public class BanquetInvoiceListModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int BanquetBookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
}

// Legacy Banquet Payment Model (embedded in booking detail)
public class BanquetPaymentModel
{
    public int Id { get; set; }
    public int BanquetBookingId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public int PaymentType { get; set; }
    public int PaymentMode { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
