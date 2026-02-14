using HMSMini.API.Models.DTOs.MenuCategory;
using HMSMini.API.Models.DTOs.MenuItem;
using HMSMini.API.Models.DTOs.MenuPackage;

namespace HMSMini.API.Services.Interfaces;

public interface IMenuService
{
    // Categories
    Task<List<MenuCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false);
    Task<MenuCategoryDto?> GetCategoryByIdAsync(int id);
    Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto);
    Task<MenuCategoryDto> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto);
    Task DeleteCategoryAsync(int id);

    // Items
    Task<List<MenuItemDto>> GetAllItemsAsync(bool includeInactive = false);
    Task<List<MenuItemDto>> GetItemsByCategoryAsync(int categoryId);
    Task<MenuItemDto?> GetItemByIdAsync(int id);
    Task<MenuItemDto> CreateItemAsync(CreateMenuItemDto dto);
    Task<MenuItemDto> UpdateItemAsync(int id, UpdateMenuItemDto dto);
    Task DeleteItemAsync(int id);

    // Packages
    Task<List<MenuPackageDto>> GetAllPackagesAsync(bool includeInactive = false);
    Task<MenuPackageDetailDto?> GetPackageByIdAsync(int id);
    Task<MenuPackageDto> CreatePackageAsync(CreateMenuPackageDto dto);
    Task<MenuPackageDto> UpdatePackageAsync(int id, UpdateMenuPackageDto dto);
    Task DeletePackageAsync(int id);
}
