using HMSMini.Web.Models;
using System.Net.Http.Json;

namespace HMSMini.Web.Services;

public interface IApiClientService
{
    // Rooms
    Task<List<RoomDto>> GetRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<List<RoomAvailabilityDto>> GetRoomAvailabilityAsync(DateTime startDate, DateTime endDate);
    Task<OccupancyReportDto?> GetOccupancyReportAsync(DateTime? date = null);
    Task<byte[]?> ExportOccupancyReportToExcelAsync(DateTime? date = null);
    Task<RoomDto?> CreateRoomAsync(CreateRoomDto room);
    Task<RoomDto?> UpdateRoomStatusAsync(int roomId, UpdateRoomStatusDto dto, int? fromStatus = null);

    // Reservations
    Task<List<ReservationDto>> GetReservationsAsync();
    Task<List<ReservationDto>> GetUpcomingReservationsAsync();
    Task<ReservationDto?> GetReservationByIdAsync(int id);
    Task<ReservationDto?> CreateReservationAsync(CreateReservationDto reservation);
    Task<ReservationDto?> ConfirmReservationAsync(int id);
    Task<ReservationDto?> AssignRoomToReservationAsync(int id, AssignRoomDto dto);
    Task<bool> CancelReservationAsync(int id);

    // Check-Ins
    Task<List<CheckInDto>> GetCheckInsAsync();
    Task<List<CheckInDto>> GetActiveCheckInsAsync();
    Task<CheckInWithGuestsDto?> GetCheckInByIdAsync(int id);
    Task<CheckInWithGuestsDto?> CreateCheckInAsync(CreateCheckInDto checkIn);
    Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInModel dto);
    Task<bool> CheckOutAsync(int id);

    // Guests
    Task<GuestDto?> GetGuestByIdAsync(int id);
    Task<List<GuestDto>> GetGuestsByCheckInIdAsync(int checkInId);
    Task<GuestDto?> AddGuestToCheckInAsync(int checkInId, CreateGuestDto guest);
    Task<GuestDto?> UpdateGuestAsync(int id, CreateGuestDto guest);
    Task<GuestDto?> UploadGuestPhotoAsync(int guestId, int photoNumber, Stream fileStream, string fileName);
    Task<OcrReviewDto?> ProcessOcrAsync(int guestId, int photoNumber);
    Task<bool> CheckOutGuestAsync(int guestId);
    Task<List<GuestDto>> SearchGuestsAsync(string query);
    Task<List<GuestDto>> SearchGuestsByMobileAsync(string mobile);
    Task<List<GuestDto>> SearchGuestsByNameAsync(string name);
    Task<OcrReviewDto?> ProcessOcrDirectAsync(Stream imageStream, string fileName);

    // Billing
    Task<BillPreviewModel?> GetBillPreviewAsync(int checkInId, DateTime? checkOutDate = null);
    Task<InvoiceModel?> FinalizeCheckoutAsync(int checkInId, FinalizeInvoiceModel dto);
    Task<InvoiceModel?> GetInvoiceAsync(int checkInId);
    Task<AdditionalChargeModel?> AddAdditionalChargeAsync(int checkInId, CreateAdditionalChargeModel dto);
    Task<List<AdditionalChargeModel>> GetAdditionalChargesAsync(int checkInId);
    Task<AdditionalChargeModel?> UpdateAdditionalChargeAsync(int chargeId, UpdateAdditionalChargeModel dto);
    Task<bool> DeleteAdditionalChargeAsync(int chargeId);

    // Room Types
    Task<List<RoomTypeDto>> GetRoomTypesAsync();
    Task<RoomTypeDto?> CreateRoomTypeAsync(CreateRoomTypeDto dto);
    Task<RoomTypeDto?> UpdateRoomTypeAsync(int id, UpdateRoomTypeDto dto);
    Task DeleteRoomTypeAsync(int id);

    // Users
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> CreateUserAsync(RegisterRequest request);
    Task<bool> DeactivateUserAsync(int id);

    // Companies
    Task<List<CompanyListDto>> GetAllCompaniesAsync(bool includeInactive = false);
    Task<List<CompanyDto>> GetActiveCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(int id);
    Task<List<CompanyDto>> SearchCompaniesAsync(string term);
    Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto);
    Task<CompanyDto?> UpdateCompanyAsync(int id, UpdateCompanyDto dto);
    Task<bool> ActivateCompanyAsync(int id);
    Task<bool> DeactivateCompanyAsync(int id);

    // Tariffs
    Task<TariffCalculationDto?> CalculateTariffAsync(int roomTypeId, int occupancy, DateTime checkIn, DateTime checkOut, int? companyId = null, int? mealPlanId = null);
    Task<List<TariffCalculationDto>> CalculateAllRoomTypesAsync(DateTime checkIn, DateTime checkOut, int? companyId = null);
    Task<List<BaseTariffDto>> GetBaseTariffsAsync();
    Task<BaseTariffDto?> GetBaseTariffByIdAsync(int id);
    Task<BaseTariffDto?> CreateBaseTariffAsync(CreateBaseTariffDto dto);
    Task<BaseTariffDto?> UpdateBaseTariffAsync(int id, UpdateBaseTariffDto dto);
    Task<List<CompanyTariffDto>> GetCompanyTariffsAsync(int companyId);
    Task<CompanyTariffDto?> CreateCompanyTariffAsync(CreateCompanyTariffDto dto);
    Task<List<CompanyTariffDto>> CreateBulkCompanyTariffsAsync(int companyId, List<CreateCompanyTariffDto> dtos);
    Task<CompanyTariffDto?> UpdateCompanyTariffAsync(int id, UpdateCompanyTariffDto dto);

    // Business Sources
    Task<List<BusinessSourceModel>> GetAllBusinessSourcesAsync();
    Task<List<BusinessSourceModel>> GetActiveBusinessSourcesAsync();
    Task<BusinessSourceModel?> GetBusinessSourceByIdAsync(int id);
    Task<BusinessSourceModel?> CreateBusinessSourceAsync(CreateBusinessSourceModel dto);
    Task<BusinessSourceModel?> UpdateBusinessSourceAsync(int id, UpdateBusinessSourceModel dto);
    Task<bool> ActivateBusinessSourceAsync(int id);
    Task<bool> DeactivateBusinessSourceAsync(int id);
    Task<bool> DeleteBusinessSourceAsync(int id);

    // Guest Types
    Task<List<GuestTypeModel>> GetAllGuestTypesAsync();
    Task<List<GuestTypeModel>> GetActiveGuestTypesAsync();
    Task<GuestTypeModel?> GetGuestTypeByIdAsync(int id);
    Task<GuestTypeModel?> CreateGuestTypeAsync(CreateGuestTypeModel dto);
    Task<GuestTypeModel?> UpdateGuestTypeAsync(int id, UpdateGuestTypeModel dto);
    Task<bool> ActivateGuestTypeAsync(int id);
    Task<bool> DeactivateGuestTypeAsync(int id);
    Task<bool> DeleteGuestTypeAsync(int id);

    // Meal Plans
    Task<List<MealPlanModel>> GetAllMealPlansAsync();
    Task<List<MealPlanModel>> GetActiveMealPlansAsync();
    Task<MealPlanModel?> GetMealPlanByIdAsync(int id);
    Task<MealPlanModel?> CreateMealPlanAsync(CreateMealPlanModel dto);
    Task<MealPlanModel?> UpdateMealPlanAsync(int id, UpdateMealPlanModel dto);
    Task<bool> ActivateMealPlanAsync(int id);
    Task<bool> DeactivateMealPlanAsync(int id);
    Task<bool> DeleteMealPlanAsync(int id);

    // Meal Plan Rates
    Task<List<MealPlanRateModel>> GetAllMealPlanRatesAsync();
    Task<List<MealPlanRateModel>> GetMealPlanRatesByMealPlanAsync(int mealPlanId);
    Task<MealPlanRateModel?> GetMealPlanRateByIdAsync(int id);
    Task<MealPlanRateModel?> CreateMealPlanRateAsync(CreateMealPlanRateModel dto);
    Task<MealPlanRateModel?> UpdateMealPlanRateAsync(int id, UpdateMealPlanRateModel dto);
    Task<bool> DeleteMealPlanRateAsync(int id);

    // Tax Slabs
    Task<List<TaxSlabModel>> GetActiveTaxSlabsAsync(DateTime? date = null);
    Task<ApplicableTaxSlabModel?> GetApplicableTaxSlabAsync(decimal amount, DateTime? date = null);

    // Voucher Tax Configurations
    Task<List<VoucherTaxConfigModel>> GetVoucherTaxConfigsAsync(bool activeOnly = false);
    Task<VoucherTaxConfigModel?> GetVoucherTaxConfigAsync(int id);
    Task<VoucherTaxConfigModel?> CreateVoucherTaxConfigAsync(CreateVoucherTaxConfigModel model);
    Task<VoucherTaxConfigModel?> UpdateVoucherTaxConfigAsync(int id, CreateVoucherTaxConfigModel model);
    Task<bool> DeleteVoucherTaxConfigAsync(int id);

    // Vouchers
    Task<VoucherModel?> GetVoucherByIdAsync(int id);
    Task<List<VoucherModel>> GetVouchersByCheckInIdAsync(int checkInId);
    Task<List<VoucherModel>> GetVouchersByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<List<VoucherSummaryModel>> GetVoucherSummaryAsync(DateTime fromDate, DateTime toDate);
    Task<VoucherModel?> CreateVoucherAsync(CreateVoucherModel model);
    Task<VoucherModel?> CancelVoucherAsync(int id, CancelVoucherModel model);

    // Day Closing
    Task<WorkingDateModel?> GetWorkingDateAsync();
    Task<DayCloseValidationModel?> ValidateDayCloseAsync();
    Task<DayClosePreviewModel?> GetDayClosePreviewAsync();
    Task<DayCloseResultModel?> CloseDayAsync();
    Task<List<DayClosingAuditModel>> GetClosingHistoryAsync(int? pageSize = null, int? skip = null);

    // Banquet Halls
    Task<List<BanquetHallModel>> GetAllBanquetHallsAsync(bool includeInactive = false);
    Task<BanquetHallModel?> GetBanquetHallByIdAsync(int id);
    Task<BanquetHallModel?> CreateBanquetHallAsync(CreateBanquetHallModel dto);
    Task<BanquetHallModel?> UpdateBanquetHallAsync(int id, UpdateBanquetHallModel dto);
    Task<bool> DeleteBanquetHallAsync(int id);
    Task<bool> ActivateBanquetHallAsync(int id);
    Task<bool> DeactivateBanquetHallAsync(int id);

    // Event Types
    Task<List<EventTypeModel>> GetAllEventTypesAsync(bool includeInactive = false);
    Task<List<EventTypeModel>> GetActiveEventTypesAsync();
    Task<EventTypeModel?> GetEventTypeByIdAsync(int id);
    Task<EventTypeModel?> CreateEventTypeAsync(CreateEventTypeModel dto);
    Task<EventTypeModel?> UpdateEventTypeAsync(int id, UpdateEventTypeModel dto);
    Task<bool> DeleteEventTypeAsync(int id);
    Task<bool> ActivateEventTypeAsync(int id);
    Task<bool> DeactivateEventTypeAsync(int id);

    // Banquet Services
    Task<List<BanquetServiceModel>> GetAllBanquetServicesAsync(bool includeInactive = false);
    Task<List<BanquetServiceModel>> GetActiveBanquetServicesAsync();
    Task<BanquetServiceModel?> GetBanquetServiceByIdAsync(int id);
    Task<BanquetServiceModel?> CreateBanquetServiceAsync(CreateBanquetServiceModel dto);
    Task<BanquetServiceModel?> UpdateBanquetServiceAsync(int id, UpdateBanquetServiceModel dto);
    Task<bool> DeleteBanquetServiceAsync(int id);
    Task<bool> ActivateBanquetServiceAsync(int id);
    Task<bool> DeactivateBanquetServiceAsync(int id);

    // Banquet Bookings
    Task<List<BanquetBookingListModel>> GetAllBanquetBookingsAsync();
    Task<BanquetBookingDetailModel?> GetBanquetBookingByIdAsync(int id);
    Task<BanquetBookingDetailModel?> CreateBanquetBookingAsync(CreateBanquetBookingModel dto);
    Task<BanquetBookingDetailModel?> UpdateBanquetBookingAsync(int id, UpdateBanquetBookingModel dto);
    Task<bool> DeleteBanquetBookingAsync(int id);
    Task<bool> UpdateBanquetBookingStatusAsync(int id, UpdateBanquetBookingStatusModel dto);

    // Booking Menus
    Task<List<BanquetBookingMenuModel>> GetBanquetBookingMenusAsync(int bookingId);
    Task<BanquetBookingMenuModel?> AddBanquetBookingMenuAsync(int bookingId, CreateBanquetBookingMenuModel dto);
    Task<BanquetBookingMenuModel?> UpdateBanquetBookingMenuAsync(int menuId, UpdateBanquetBookingMenuModel dto);
    Task<bool> DeleteBanquetBookingMenuAsync(int menuId);

    // Booking Services
    Task<List<BanquetBookingServiceModel>> GetBanquetBookingServicesAsync(int bookingId);
    Task<BanquetBookingServiceModel?> AddBanquetBookingServiceAsync(int bookingId, CreateBanquetBookingServiceModel dto);
    Task<BanquetBookingServiceModel?> UpdateBanquetBookingServiceAsync(int serviceId, UpdateBanquetBookingServiceModel dto);
    Task<bool> DeleteBanquetBookingServiceAsync(int serviceId);

    // Banquet Charges
    Task<List<BanquetChargeModel>> GetBanquetChargesByBookingAsync(int bookingId);
    Task<BanquetChargeModel?> CreateBanquetChargeAsync(int bookingId, CreateBanquetChargeModel dto);
    Task<BanquetChargeModel?> UpdateBanquetChargeAsync(int chargeId, UpdateBanquetChargeModel dto);
    Task<bool> DeleteBanquetChargeAsync(int chargeId);

    // Banquet Billing
    Task<BanquetBillPreviewModel?> GetBanquetBillPreviewAsync(int bookingId);
    Task<BanquetInvoiceModel?> FinalizeBanquetInvoiceAsync(int bookingId, FinalizeBanquetInvoiceModel dto);
    Task<BanquetInvoiceModel?> GetBanquetInvoiceByIdAsync(int invoiceId);
    Task<BanquetInvoiceModel?> GetBanquetInvoiceByBookingAsync(int bookingId);

    // Unified Payments
    Task<PaymentModel?> CreatePaymentAsync(CreatePaymentModel dto);
    Task<PaymentModel?> GetPaymentByIdAsync(int id);
    Task<List<PaymentModel>> GetPaymentsByBanquetAsync(int bookingId);
    Task<PaymentSummaryModel?> GetBanquetPaymentSummaryAsync(int bookingId);
    Task<List<PaymentModel>> GetPaymentsByCheckInAsync(int checkInId);
    Task<PaymentSummaryModel?> GetCheckInPaymentSummaryAsync(int checkInId);
    Task<bool> CancelPaymentAsync(int id, string? reason = null);

    // Financial Reports
    Task<CompanyOutstandingListModel?> GetCompanyOutstandingAsync(DateTime? asOfDate = null);
    Task<CompanyOutstandingSummaryModel?> GetCompanyOutstandingByIdAsync(int companyId, DateTime? asOfDate = null);
    Task<CompanyStatementModel?> GetCompanyStatementAsync(int companyId, DateTime from, DateTime to);
    Task<AgingReportModel?> GetAgingReportAsync(DateTime? asOfDate = null);
    Task<BusinessSourceOutstandingModel?> GetBusinessSourceOutstandingAsync(DateTime? asOfDate = null);

    // Utility
    string GetBaseUrl();
}

public class ApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;

    public ApiClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Rooms
    public async Task<List<RoomDto>> GetRoomsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoomDto>>("/api/rooms") ?? new();
        }
        catch { return new(); }
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RoomDto>($"/api/rooms/{id}");
        }
        catch { return null; }
    }

    public async Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        try
        {
            var checkInStr = checkIn.ToString("yyyy-MM-dd");
            var checkOutStr = checkOut.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<RoomDto>>(
                $"/api/rooms/available?checkIn={checkInStr}&checkOut={checkOutStr}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<RoomAvailabilityDto>> GetRoomAvailabilityAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var startDateStr = startDate.ToString("yyyy-MM-dd");
            var endDateStr = endDate.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<RoomAvailabilityDto>>(
                $"/api/rooms/availability?startDate={startDateStr}&endDate={endDateStr}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<OccupancyReportDto?> GetOccupancyReportAsync(DateTime? date = null)
    {
        try
        {
            var reportDate = date ?? DateTime.Today;
            var dateStr = reportDate.ToString("yyyy-MM-dd");
            var result = await _httpClient.GetFromJsonAsync<OccupancyReportDto>(
                $"/api/rooms/occupancy-report?date={dateStr}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetOccupancyReportAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<byte[]?> ExportOccupancyReportToExcelAsync(DateTime? date = null)
    {
        try
        {
            var reportDate = date ?? DateTime.Today;
            var dateStr = reportDate.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"/api/rooms/occupancy-report/export?date={dateStr}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            Console.WriteLine($"Excel export failed with status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in ExportOccupancyReportToExcelAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<RoomDto?> CreateRoomAsync(CreateRoomDto room)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/rooms", room);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<RoomDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<RoomDto?> UpdateRoomStatusAsync(int roomId, UpdateRoomStatusDto dto, int? fromStatus = null)
    {
        try
        {
            // Determine which endpoint to use based on the status transition
            string endpoint;

            // Maintenance endpoint - for setting to/from Maintenance status
            if (dto.RoomStatus == 3 || fromStatus == 3)
            {
                endpoint = $"/api/rooms/{roomId}/maintenance-status";
            }
            // Housekeeping endpoint - for Available (0) and Dirty (2) cleaning operations
            else if (dto.RoomStatus == 0 || dto.RoomStatus == 2)
            {
                endpoint = $"/api/rooms/{roomId}/cleaning-status";
            }
            // General status endpoint - for Blocked (4) and other admin operations
            else
            {
                endpoint = $"/api/rooms/{roomId}/status";
            }

            var response = await _httpClient.PutAsJsonAsync(endpoint, dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<RoomDto>()
                : null;
        }
        catch { return null; }
    }

    // Reservations
    public async Task<List<ReservationDto>> GetReservationsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>("/api/reservations") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<ReservationDto>> GetUpcomingReservationsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>("/api/reservations/upcoming") ?? new();
        }
        catch { return new(); }
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ReservationDto>($"/api/reservations/{id}");
        }
        catch { return null; }
    }

    public async Task<ReservationDto?> CreateReservationAsync(CreateReservationDto reservation)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/reservations", reservation);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<ReservationDto?> ConfirmReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/reservations/{id}/confirm", null);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<ReservationDto?> AssignRoomToReservationAsync(int id, AssignRoomDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/reservations/{id}/assign-room", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ReservationDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/reservations/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Check-Ins
    public async Task<List<CheckInDto>> GetCheckInsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CheckInDto>>("/api/checkins") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<CheckInDto>> GetActiveCheckInsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CheckInDto>>("/api/checkins/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<CheckInWithGuestsDto?> GetCheckInByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CheckInWithGuestsDto>($"/api/checkins/{id}");
        }
        catch { return null; }
    }

    public async Task<CheckInWithGuestsDto?> CreateCheckInAsync(CreateCheckInDto checkIn)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/checkins", checkIn);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CheckInWithGuestsDto>();
            }
            else
            {
                // Read and log the error response for debugging
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"=== API Error {response.StatusCode} ===");
                Console.WriteLine(errorContent);
                Console.WriteLine("=== End API Error ===");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating check-in: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/checkins/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CheckInDto>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> CheckOutAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/checkins/{id}/checkout", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Billing
    public async Task<BillPreviewModel?> GetBillPreviewAsync(int checkInId, DateTime? checkOutDate = null)
    {
        try
        {
            var url = $"/api/checkins/{checkInId}/bill-preview";
            if (checkOutDate.HasValue)
            {
                url += $"?checkOutDate={checkOutDate.Value:yyyy-MM-ddTHH:mm:ss}";
            }
            return await _httpClient.GetFromJsonAsync<BillPreviewModel>(url);
        }
        catch { return null; }
    }

    public async Task<InvoiceModel?> FinalizeCheckoutAsync(int checkInId, FinalizeInvoiceModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/checkins/{checkInId}/finalize-checkout", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<InvoiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<InvoiceModel?> GetInvoiceAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<InvoiceModel>($"/api/checkins/{checkInId}/invoice");
        }
        catch { return null; }
    }

    public async Task<AdditionalChargeModel?> AddAdditionalChargeAsync(int checkInId, CreateAdditionalChargeModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/billing/checkin/{checkInId}/charges", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<AdditionalChargeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<List<AdditionalChargeModel>> GetAdditionalChargesAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<AdditionalChargeModel>>($"/api/billing/checkin/{checkInId}/charges") ?? new();
        }
        catch { return new(); }
    }

    public async Task<AdditionalChargeModel?> UpdateAdditionalChargeAsync(int chargeId, UpdateAdditionalChargeModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/billing/charges/{chargeId}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<AdditionalChargeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteAdditionalChargeAsync(int chargeId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/billing/charges/{chargeId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Guests
    public async Task<GuestDto?> GetGuestByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GuestDto>($"/api/guests/{id}");
        }
        catch { return null; }
    }

    public async Task<List<GuestDto>> GetGuestsByCheckInIdAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestDto>>($"/api/guests/checkin/{checkInId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<GuestDto?> AddGuestToCheckInAsync(int checkInId, CreateGuestDto guest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/guests/checkin/{checkInId}", guest);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<GuestDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<GuestDto?> UpdateGuestAsync(int id, CreateGuestDto guest)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/guests/{id}", guest);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<GuestDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<GuestDto?> UploadGuestPhotoAsync(int guestId, int photoNumber, Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent(photoNumber.ToString()), "photoNumber");

            var response = await _httpClient.PostAsync($"/api/guests/{guestId}/upload-photo", content);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<GuestDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<OcrReviewDto?> ProcessOcrAsync(int guestId, int photoNumber)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/guests/{guestId}/process-ocr?photoNumber={photoNumber}", null);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<OcrReviewDto>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> CheckOutGuestAsync(int guestId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/guests/{guestId}/checkout", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Guest Search
    public async Task<List<GuestDto>> SearchGuestsAsync(string query)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestDto>>($"/api/guests/search?query={Uri.EscapeDataString(query)}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<GuestDto>> SearchGuestsByMobileAsync(string mobile)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestDto>>($"/api/guests/search/mobile?mobile={Uri.EscapeDataString(mobile)}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<GuestDto>> SearchGuestsByNameAsync(string name)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestDto>>($"/api/guests/search/name?name={Uri.EscapeDataString(name)}") ?? new();
        }
        catch { return new(); }
    }

    // Direct OCR Processing
    public async Task<OcrReviewDto?> ProcessOcrDirectAsync(Stream imageStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(imageStream);

            // Determine content type from file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("/api/guests/ocr/direct", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OCR API Error ({response.StatusCode}): {errorContent}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<OcrReviewDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in ProcessOcrDirectAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    // Room Types
    public async Task<List<RoomTypeDto>> GetRoomTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<RoomTypeDto>>("/api/roomtypes") ?? new();
        }
        catch { return new(); }
    }

    public async Task<RoomTypeDto?> CreateRoomTypeAsync(CreateRoomTypeDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/roomtypes", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RoomTypeDto>();
    }

    public async Task<RoomTypeDto?> UpdateRoomTypeAsync(int id, UpdateRoomTypeDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/roomtypes/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RoomTypeDto>();
    }

    public async Task DeleteRoomTypeAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/roomtypes/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Users
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/auth") ?? new();
        }
        catch { return new(); }
    }

    public async Task<UserDto?> CreateUserAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    return new UserDto
                    {
                        Id = authResponse.Id,
                        Username = authResponse.Username,
                        Email = authResponse.Email,
                        FullName = authResponse.FullName,
                        Role = authResponse.Role,
                        IsActive = true
                    };
                }
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/auth/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Companies
    public async Task<List<CompanyListDto>> GetAllCompaniesAsync(bool includeInactive = false)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CompanyListDto>>(
                $"/api/companies?includeInactive={includeInactive}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<CompanyDto>> GetActiveCompaniesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CompanyDto>>("/api/companies/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CompanyDto>($"/api/companies/{id}");
        }
        catch { return null; }
    }

    public async Task<List<CompanyDto>> SearchCompaniesAsync(string term)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CompanyDto>>(
                $"/api/companies/search?term={Uri.EscapeDataString(term)}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<CompanyDto?> CreateCompanyAsync(CreateCompanyDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/companies", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CompanyDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to create company: {response.StatusCode} - {error}");
    }

    public async Task<CompanyDto?> UpdateCompanyAsync(int id, UpdateCompanyDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/companies/{id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CompanyDto>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Failed to update company: {response.StatusCode} - {error}");
    }

    public async Task<bool> ActivateCompanyAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/companies/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateCompanyAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/companies/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Tariffs
    public async Task<TariffCalculationDto?> CalculateTariffAsync(
        int roomTypeId, int occupancy, DateTime checkIn, DateTime checkOut, int? companyId = null, int? mealPlanId = null)
    {
        try
        {
            var checkInStr = checkIn.ToString("yyyy-MM-dd");
            var checkOutStr = checkOut.ToString("yyyy-MM-dd");
            var companyParam = companyId.HasValue ? $"&companyId={companyId.Value}" : "";
            var mealPlanParam = mealPlanId.HasValue ? $"&mealPlanId={mealPlanId.Value}" : "";

            return await _httpClient.GetFromJsonAsync<TariffCalculationDto>(
                $"/api/tariffs/calculate?roomTypeId={roomTypeId}&occupancy={occupancy}&checkIn={checkInStr}&checkOut={checkOutStr}{companyParam}{mealPlanParam}");
        }
        catch { return null; }
    }

    public async Task<List<TariffCalculationDto>> CalculateAllRoomTypesAsync(
        DateTime checkIn, DateTime checkOut, int? companyId = null)
    {
        try
        {
            var checkInStr = checkIn.ToString("yyyy-MM-dd");
            var checkOutStr = checkOut.ToString("yyyy-MM-dd");
            var companyParam = companyId.HasValue ? $"&companyId={companyId.Value}" : "";

            return await _httpClient.GetFromJsonAsync<List<TariffCalculationDto>>(
                $"/api/tariffs/calculate-all?checkIn={checkInStr}&checkOut={checkOutStr}{companyParam}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<BaseTariffDto>> GetBaseTariffsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BaseTariffDto>>("/api/tariffs/base") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BaseTariffDto?> GetBaseTariffByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BaseTariffDto>($"/api/tariffs/base/{id}");
        }
        catch { return null; }
    }

    public async Task<BaseTariffDto?> CreateBaseTariffAsync(CreateBaseTariffDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/tariffs/base", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BaseTariffDto>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<BaseTariffDto?> UpdateBaseTariffAsync(int id, UpdateBaseTariffDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/tariffs/base/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BaseTariffDto>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<List<CompanyTariffDto>> GetCompanyTariffsAsync(int companyId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CompanyTariffDto>>(
                $"/api/tariffs/company/{companyId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<CompanyTariffDto?> CreateCompanyTariffAsync(CreateCompanyTariffDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/tariffs/company", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CompanyTariffDto>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<List<CompanyTariffDto>> CreateBulkCompanyTariffsAsync(
        int companyId, List<CreateCompanyTariffDto> dtos)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/tariffs/company/bulk?companyId={companyId}", dtos);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CompanyTariffDto>>() ?? new();
            }
            return new();
        }
        catch { return new(); }
    }

    public async Task<CompanyTariffDto?> UpdateCompanyTariffAsync(int id, UpdateCompanyTariffDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/tariffs/company/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CompanyTariffDto>();
            }
            return null;
        }
        catch { return null; }
    }

    // Business Sources
    public async Task<List<BusinessSourceModel>> GetAllBusinessSourcesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BusinessSourceModel>>("/api/business-sources") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<BusinessSourceModel>> GetActiveBusinessSourcesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BusinessSourceModel>>("/api/business-sources/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BusinessSourceModel?> GetBusinessSourceByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BusinessSourceModel>($"/api/business-sources/{id}");
        }
        catch { return null; }
    }

    public async Task<BusinessSourceModel?> CreateBusinessSourceAsync(CreateBusinessSourceModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/business-sources", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BusinessSourceModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<BusinessSourceModel?> UpdateBusinessSourceAsync(int id, UpdateBusinessSourceModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/business-sources/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BusinessSourceModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> ActivateBusinessSourceAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/business-sources/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateBusinessSourceAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/business-sources/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteBusinessSourceAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/business-sources/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Guest Types
    public async Task<List<GuestTypeModel>> GetAllGuestTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestTypeModel>>("/api/guesttypes") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<GuestTypeModel>> GetActiveGuestTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<GuestTypeModel>>("/api/guesttypes/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<GuestTypeModel?> GetGuestTypeByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GuestTypeModel>($"/api/guesttypes/{id}");
        }
        catch { return null; }
    }

    public async Task<GuestTypeModel?> CreateGuestTypeAsync(CreateGuestTypeModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/guesttypes", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GuestTypeModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<GuestTypeModel?> UpdateGuestTypeAsync(int id, UpdateGuestTypeModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/guesttypes/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GuestTypeModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> ActivateGuestTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/guesttypes/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateGuestTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/guesttypes/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteGuestTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/guesttypes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Meal Plans
    public async Task<List<MealPlanModel>> GetAllMealPlansAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<MealPlanModel>>("/api/meal-plans") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<MealPlanModel>> GetActiveMealPlansAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<MealPlanModel>>("/api/meal-plans/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<MealPlanModel?> GetMealPlanByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MealPlanModel>($"/api/meal-plans/{id}");
        }
        catch { return null; }
    }

    public async Task<MealPlanModel?> CreateMealPlanAsync(CreateMealPlanModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/meal-plans", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MealPlanModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<MealPlanModel?> UpdateMealPlanAsync(int id, UpdateMealPlanModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/meal-plans/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MealPlanModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> ActivateMealPlanAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/meal-plans/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateMealPlanAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/meal-plans/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteMealPlanAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/meal-plans/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Meal Plan Rates
    public async Task<List<MealPlanRateModel>> GetAllMealPlanRatesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<MealPlanRateModel>>("/api/meal-plan-rates") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<MealPlanRateModel>> GetMealPlanRatesByMealPlanAsync(int mealPlanId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<MealPlanRateModel>>($"/api/meal-plan-rates/by-meal-plan/{mealPlanId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<MealPlanRateModel?> GetMealPlanRateByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MealPlanRateModel>($"/api/meal-plan-rates/{id}");
        }
        catch { return null; }
    }

    public async Task<MealPlanRateModel?> CreateMealPlanRateAsync(CreateMealPlanRateModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/meal-plan-rates", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MealPlanRateModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<MealPlanRateModel?> UpdateMealPlanRateAsync(int id, UpdateMealPlanRateModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/meal-plan-rates/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MealPlanRateModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteMealPlanRateAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/meal-plan-rates/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Tax Slabs
    public async Task<List<TaxSlabModel>> GetActiveTaxSlabsAsync(DateTime? date = null)
    {
        try
        {
            var url = date.HasValue
                ? $"/api/taxslabs?date={date.Value:yyyy-MM-dd}"
                : "/api/taxslabs";

            return await _httpClient.GetFromJsonAsync<List<TaxSlabModel>>(url) ?? new();
        }
        catch { return new(); }
    }

    public async Task<ApplicableTaxSlabModel?> GetApplicableTaxSlabAsync(decimal amount, DateTime? date = null)
    {
        try
        {
            var url = date.HasValue
                ? $"/api/taxslabs/applicable?amount={amount}&date={date.Value:yyyy-MM-dd}"
                : $"/api/taxslabs/applicable?amount={amount}";

            return await _httpClient.GetFromJsonAsync<ApplicableTaxSlabModel>(url);
        }
        catch { return null; }
    }

    // Voucher Tax Configurations
    public async Task<List<VoucherTaxConfigModel>> GetVoucherTaxConfigsAsync(bool activeOnly = false)
    {
        try
        {
            var url = activeOnly ? "/api/vouchertaxconfigurations?activeOnly=true" : "/api/vouchertaxconfigurations";
            return await _httpClient.GetFromJsonAsync<List<VoucherTaxConfigModel>>(url) ?? new();
        }
        catch { return new(); }
    }

    public async Task<VoucherTaxConfigModel?> GetVoucherTaxConfigAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<VoucherTaxConfigModel>($"/api/vouchertaxconfigurations/{id}");
        }
        catch { return null; }
    }

    public async Task<VoucherTaxConfigModel?> CreateVoucherTaxConfigAsync(CreateVoucherTaxConfigModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/vouchertaxconfigurations", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VoucherTaxConfigModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<VoucherTaxConfigModel?> UpdateVoucherTaxConfigAsync(int id, CreateVoucherTaxConfigModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/vouchertaxconfigurations/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VoucherTaxConfigModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteVoucherTaxConfigAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/vouchertaxconfigurations/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Vouchers
    public async Task<VoucherModel?> GetVoucherByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<VoucherModel>($"/api/vouchers/{id}");
        }
        catch { return null; }
    }

    public async Task<List<VoucherModel>> GetVouchersByCheckInIdAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<VoucherModel>>($"/api/vouchers/check-in/{checkInId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<VoucherModel>> GetVouchersByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var fromDateStr = fromDate.ToString("yyyy-MM-dd");
            var toDateStr = toDate.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<VoucherModel>>(
                $"/api/vouchers/date-range?fromDate={fromDateStr}&toDate={toDateStr}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<VoucherSummaryModel>> GetVoucherSummaryAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var fromDateStr = fromDate.ToString("yyyy-MM-dd");
            var toDateStr = toDate.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<VoucherSummaryModel>>(
                $"/api/vouchers/summary?fromDate={fromDateStr}&toDate={toDateStr}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<VoucherModel?> CreateVoucherAsync(CreateVoucherModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/vouchers", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VoucherModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<VoucherModel?> CancelVoucherAsync(int id, CancelVoucherModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/vouchers/{id}/cancel", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VoucherModel>();
            }
            return null;
        }
        catch { return null; }
    }

    // Day Closing
    public async Task<WorkingDateModel?> GetWorkingDateAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WorkingDateModel>("/api/day-closing/working-date");
        }
        catch { return null; }
    }

    public async Task<DayCloseValidationModel?> ValidateDayCloseAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DayCloseValidationModel>("/api/day-closing/validate");
        }
        catch { return null; }
    }

    public async Task<DayClosePreviewModel?> GetDayClosePreviewAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DayClosePreviewModel>("/api/day-closing/preview");
        }
        catch { return null; }
    }

    public async Task<DayCloseResultModel?> CloseDayAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/day-closing/close", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DayCloseResultModel>();
            }
            return null;
        }
        catch { return null; }
    }

    public async Task<List<DayClosingAuditModel>> GetClosingHistoryAsync(int? pageSize = null, int? skip = null)
    {
        try
        {
            var pageSizeParam = pageSize.HasValue ? $"pageSize={pageSize.Value}" : "";
            var skipParam = skip.HasValue ? $"skip={skip.Value}" : "";
            var query = string.Join("&", new[] { pageSizeParam, skipParam }.Where(x => !string.IsNullOrEmpty(x)));
            var url = string.IsNullOrEmpty(query) ? "/api/day-closing/history" : $"/api/day-closing/history?{query}";

            return await _httpClient.GetFromJsonAsync<List<DayClosingAuditModel>>(url) ?? new();
        }
        catch { return new(); }
    }

    // Banquet Halls
    public async Task<List<BanquetHallModel>> GetAllBanquetHallsAsync(bool includeInactive = false)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetHallModel>>(
                $"/api/banquet-halls?includeInactive={includeInactive}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetHallModel?> GetBanquetHallByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetHallModel>($"/api/banquet-halls/{id}");
        }
        catch { return null; }
    }

    public async Task<BanquetHallModel?> CreateBanquetHallAsync(CreateBanquetHallModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/banquet-halls", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetHallModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetHallModel?> UpdateBanquetHallAsync(int id, UpdateBanquetHallModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-halls/{id}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetHallModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetHallAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-halls/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ActivateBanquetHallAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/banquet-halls/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateBanquetHallAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/banquet-halls/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Event Types
    public async Task<List<EventTypeModel>> GetAllEventTypesAsync(bool includeInactive = false)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<EventTypeModel>>(
                $"/api/event-types?includeInactive={includeInactive}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<EventTypeModel>> GetActiveEventTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<EventTypeModel>>("/api/event-types/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<EventTypeModel?> GetEventTypeByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EventTypeModel>($"/api/event-types/{id}");
        }
        catch { return null; }
    }

    public async Task<EventTypeModel?> CreateEventTypeAsync(CreateEventTypeModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/event-types", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<EventTypeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<EventTypeModel?> UpdateEventTypeAsync(int id, UpdateEventTypeModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/event-types/{id}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<EventTypeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteEventTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/event-types/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ActivateEventTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/event-types/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateEventTypeAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/event-types/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Banquet Services
    public async Task<List<BanquetServiceModel>> GetAllBanquetServicesAsync(bool includeInactive = false)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetServiceModel>>(
                $"/api/banquet-services?includeInactive={includeInactive}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<BanquetServiceModel>> GetActiveBanquetServicesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetServiceModel>>("/api/banquet-services/active") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetServiceModel?> GetBanquetServiceByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetServiceModel>($"/api/banquet-services/{id}");
        }
        catch { return null; }
    }

    public async Task<BanquetServiceModel?> CreateBanquetServiceAsync(CreateBanquetServiceModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/banquet-services", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetServiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetServiceModel?> UpdateBanquetServiceAsync(int id, UpdateBanquetServiceModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-services/{id}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetServiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetServiceAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-services/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ActivateBanquetServiceAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/banquet-services/{id}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateBanquetServiceAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/banquet-services/{id}/deactivate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Banquet Bookings
    public async Task<List<BanquetBookingListModel>> GetAllBanquetBookingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetBookingListModel>>("/api/banquet-bookings") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetBookingDetailModel?> GetBanquetBookingByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetBookingDetailModel>($"/api/banquet-bookings/{id}");
        }
        catch { return null; }
    }

    public async Task<BanquetBookingDetailModel?> CreateBanquetBookingAsync(CreateBanquetBookingModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/banquet-bookings", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BanquetBookingDetailModel>();
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create booking error ({response.StatusCode}): {errorContent}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating banquet booking: {ex.Message}");
            return null;
        }
    }

    public async Task<BanquetBookingDetailModel?> UpdateBanquetBookingAsync(int id, UpdateBanquetBookingModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-bookings/{id}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetBookingDetailModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetBookingAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-bookings/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateBanquetBookingStatusAsync(int id, UpdateBanquetBookingStatusModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-bookings/{id}/status", dto);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Booking Menus
    public async Task<List<BanquetBookingMenuModel>> GetBanquetBookingMenusAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetBookingMenuModel>>(
                $"/api/banquet-bookings/{bookingId}/menus") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetBookingMenuModel?> AddBanquetBookingMenuAsync(int bookingId, CreateBanquetBookingMenuModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/banquet-bookings/{bookingId}/menus", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetBookingMenuModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetBookingMenuModel?> UpdateBanquetBookingMenuAsync(int menuId, UpdateBanquetBookingMenuModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-bookings/menus/{menuId}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetBookingMenuModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetBookingMenuAsync(int menuId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-bookings/menus/{menuId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Booking Services
    public async Task<List<BanquetBookingServiceModel>> GetBanquetBookingServicesAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetBookingServiceModel>>(
                $"/api/banquet-bookings/{bookingId}/services") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetBookingServiceModel?> AddBanquetBookingServiceAsync(int bookingId, CreateBanquetBookingServiceModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/banquet-bookings/{bookingId}/services", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetBookingServiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetBookingServiceModel?> UpdateBanquetBookingServiceAsync(int serviceId, UpdateBanquetBookingServiceModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-bookings/services/{serviceId}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetBookingServiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetBookingServiceAsync(int serviceId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-bookings/services/{serviceId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Banquet Charges
    public async Task<List<BanquetChargeModel>> GetBanquetChargesByBookingAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BanquetChargeModel>>(
                $"/api/banquet-charges/booking/{bookingId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<BanquetChargeModel?> CreateBanquetChargeAsync(int bookingId, CreateBanquetChargeModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/banquet-charges/booking/{bookingId}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetChargeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetChargeModel?> UpdateBanquetChargeAsync(int chargeId, UpdateBanquetChargeModel dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/banquet-charges/{chargeId}", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetChargeModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteBanquetChargeAsync(int chargeId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/banquet-charges/{chargeId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Banquet Billing
    public async Task<BanquetBillPreviewModel?> GetBanquetBillPreviewAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetBillPreviewModel>(
                $"/api/banquet-billing/booking/{bookingId}/preview");
        }
        catch { return null; }
    }

    public async Task<BanquetInvoiceModel?> FinalizeBanquetInvoiceAsync(int bookingId, FinalizeBanquetInvoiceModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/banquet-billing/booking/{bookingId}/finalize", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<BanquetInvoiceModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<BanquetInvoiceModel?> GetBanquetInvoiceByIdAsync(int invoiceId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetInvoiceModel>(
                $"/api/banquet-billing/invoice/{invoiceId}");
        }
        catch { return null; }
    }

    public async Task<BanquetInvoiceModel?> GetBanquetInvoiceByBookingAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BanquetInvoiceModel>(
                $"/api/banquet-billing/booking/{bookingId}/invoice");
        }
        catch { return null; }
    }

    // Unified Payments
    public async Task<PaymentModel?> CreatePaymentAsync(CreatePaymentModel dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/payments", dto);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<PaymentModel>()
                : null;
        }
        catch { return null; }
    }

    public async Task<PaymentModel?> GetPaymentByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PaymentModel>($"/api/payments/{id}");
        }
        catch { return null; }
    }

    public async Task<List<PaymentModel>> GetPaymentsByBanquetAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PaymentModel>>(
                $"/api/payments/banquet/{bookingId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<PaymentSummaryModel?> GetBanquetPaymentSummaryAsync(int bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PaymentSummaryModel>(
                $"/api/payments/banquet/{bookingId}/summary");
        }
        catch { return null; }
    }

    public async Task<List<PaymentModel>> GetPaymentsByCheckInAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<PaymentModel>>(
                $"/api/payments/checkin/{checkInId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<PaymentSummaryModel?> GetCheckInPaymentSummaryAsync(int checkInId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PaymentSummaryModel>(
                $"/api/payments/checkin/{checkInId}/summary");
        }
        catch { return null; }
    }

    public async Task<bool> CancelPaymentAsync(int id, string? reason = null)
    {
        try
        {
            var url = $"/api/payments/{id}/cancel";
            if (!string.IsNullOrEmpty(reason))
                url += $"?reason={Uri.EscapeDataString(reason)}";
            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // Financial Reports
    public async Task<CompanyOutstandingListModel?> GetCompanyOutstandingAsync(DateTime? asOfDate = null)
    {
        try
        {
            var url = "/api/financial-reports/company-outstanding";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<CompanyOutstandingListModel>(url);
        }
        catch { return null; }
    }

    public async Task<CompanyOutstandingSummaryModel?> GetCompanyOutstandingByIdAsync(int companyId, DateTime? asOfDate = null)
    {
        try
        {
            var url = $"/api/financial-reports/company-outstanding/{companyId}";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<CompanyOutstandingSummaryModel>(url);
        }
        catch { return null; }
    }

    public async Task<CompanyStatementModel?> GetCompanyStatementAsync(int companyId, DateTime from, DateTime to)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CompanyStatementModel>(
                $"/api/financial-reports/company-statement/{companyId}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        }
        catch { return null; }
    }

    public async Task<AgingReportModel?> GetAgingReportAsync(DateTime? asOfDate = null)
    {
        try
        {
            var url = "/api/financial-reports/aging";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<AgingReportModel>(url);
        }
        catch { return null; }
    }

    public async Task<BusinessSourceOutstandingModel?> GetBusinessSourceOutstandingAsync(DateTime? asOfDate = null)
    {
        try
        {
            var url = "/api/financial-reports/business-source-outstanding";
            if (asOfDate.HasValue)
                url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<BusinessSourceOutstandingModel>(url);
        }
        catch { return null; }
    }

    public string GetBaseUrl()
    {
        return _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
    }
}
