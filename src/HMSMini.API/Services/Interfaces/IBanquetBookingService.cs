using HMSMini.API.Models.DTOs.BanquetBooking;
using HMSMini.API.Models.DTOs.BanquetBookingMenu;
using HMSMini.API.Models.DTOs.BanquetBookingService;

namespace HMSMini.API.Services.Interfaces;

public interface IBanquetBookingService
{
    Task<List<BanquetBookingListDto>> GetAllAsync();
    Task<BanquetBookingDetailDto?> GetByIdAsync(int id);
    Task<BanquetBookingDto> CreateAsync(CreateBanquetBookingDto dto);
    Task<BanquetBookingDto> UpdateAsync(int id, UpdateBanquetBookingDto dto);
    Task DeleteAsync(int id);
    Task<BanquetBookingDto> UpdateStatusAsync(int id, UpdateBanquetBookingStatusDto dto);

    // Menu management
    Task<List<BanquetBookingMenuDto>> GetMenusByBookingAsync(int bookingId);
    Task<BanquetBookingMenuDto> AddMenuAsync(int bookingId, CreateBanquetBookingMenuDto dto);
    Task<BanquetBookingMenuDto> UpdateMenuAsync(int menuId, UpdateBanquetBookingMenuDto dto);
    Task DeleteMenuAsync(int menuId);

    // Service management
    Task<List<BanquetBookingServiceDto>> GetServicesByBookingAsync(int bookingId);
    Task<BanquetBookingServiceDto> AddServiceAsync(int bookingId, CreateBanquetBookingServiceDto dto);
    Task<BanquetBookingServiceDto> UpdateServiceAsync(int serviceId, UpdateBanquetBookingServiceDto dto);
    Task DeleteServiceAsync(int serviceId);
}
