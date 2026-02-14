using HMSMini.API.Models.DTOs.Tariff;

namespace HMSMini.API.Services.Interfaces;

/// <summary>
/// Service interface for tariff management and calculation
/// </summary>
public interface ITariffService
{
    // Tariff Calculation
    Task<TariffCalculationDto> CalculateTariffAsync(
        int roomTypeId,
        int occupancyCount,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? companyId = null,
        int? mealPlanId = null);

    Task<List<TariffCalculationDto>> CalculateAllRoomTypesAsync(
        DateTime checkInDate,
        DateTime checkOutDate,
        int? companyId = null,
        int? mealPlanId = null);

    // Base Tariff Management
    Task<List<BaseTariffDto>> GetBaseTariffsAsync();
    Task<BaseTariffDto?> GetBaseTariffByIdAsync(int id);
    Task<List<BaseTariffDto>> GetBaseTariffsByRoomTypeAsync(int roomTypeId);
    Task<BaseTariffDto> CreateBaseTariffAsync(CreateBaseTariffDto dto);
    Task<BaseTariffDto> UpdateBaseTariffAsync(int id, UpdateBaseTariffDto dto);
    Task DeleteBaseTariffAsync(int id);

    // Company Tariff Management
    Task<List<CompanyTariffDto>> GetCompanyTariffsAsync(int companyId);
    Task<CompanyTariffDto?> GetCompanyTariffByIdAsync(int id);
    Task<CompanyTariffDto> CreateCompanyTariffAsync(CreateCompanyTariffDto dto);
    Task<CompanyTariffDto> UpdateCompanyTariffAsync(int id, UpdateCompanyTariffDto dto);
    Task DeleteCompanyTariffAsync(int id);
    Task<List<CompanyTariffDto>> CreateBulkCompanyTariffsAsync(int companyId, List<CreateCompanyTariffDto> dtos);
}
