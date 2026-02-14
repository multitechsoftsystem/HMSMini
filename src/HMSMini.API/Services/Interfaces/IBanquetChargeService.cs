using HMSMini.API.Models.DTOs.BanquetCharge;

namespace HMSMini.API.Services.Interfaces;

public interface IBanquetChargeService
{
    Task<List<BanquetChargeDto>> GetByBookingAsync(int bookingId);
    Task<BanquetChargeDto> CreateAsync(int bookingId, CreateBanquetChargeDto dto);
    Task<BanquetChargeDto> UpdateAsync(int chargeId, UpdateBanquetChargeDto dto);
    Task DeleteAsync(int chargeId);
}
