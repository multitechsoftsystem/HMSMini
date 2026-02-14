using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.MenuCategory;
using HMSMini.API.Models.DTOs.MenuItem;
using HMSMini.API.Models.DTOs.MenuPackage;
using HMSMini.API.Models.Entities;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MenuService> _logger;

    public MenuService(ApplicationDbContext context, ILogger<MenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // === Categories ===

    public async Task<List<MenuCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false)
    {
        var query = _context.MenuCategories.Where(c => c.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query.Select(c => new MenuCategoryDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).OrderBy(c => c.CategoryName).ToListAsync();
    }

    public async Task<MenuCategoryDto?> GetCategoryByIdAsync(int id)
    {
        return await _context.MenuCategories
            .Where(c => c.Id == id && c.DeletedAt == null)
            .Select(c => new MenuCategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto)
    {
        var entity = new MMenuCategory
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            IsActive = true
        };

        _context.MenuCategories.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetCategoryByIdAsync(entity.Id))!;
    }

    public async Task<MenuCategoryDto> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto)
    {
        var entity = await _context.MenuCategories.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuCategory), id);

        entity.CategoryName = dto.CategoryName;
        entity.Description = dto.Description;

        await _context.SaveChangesAsync();
        return (await GetCategoryByIdAsync(id))!;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var entity = await _context.MenuCategories.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuCategory), id);

        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // === Items ===

    public async Task<List<MenuItemDto>> GetAllItemsAsync(bool includeInactive = false)
    {
        var query = _context.MenuItems
            .Include(i => i.MenuCategory)
            .Where(i => i.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(i => i.IsActive);

        return await query.Select(i => new MenuItemDto
        {
            Id = i.Id,
            MenuCategoryId = i.MenuCategoryId,
            CategoryName = i.MenuCategory.CategoryName,
            ItemName = i.ItemName,
            ItemType = i.ItemType,
            PricePerPlate = i.PricePerPlate,
            IsActive = i.IsActive,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).OrderBy(i => i.CategoryName).ThenBy(i => i.ItemName).ToListAsync();
    }

    public async Task<List<MenuItemDto>> GetItemsByCategoryAsync(int categoryId)
    {
        return await _context.MenuItems
            .Include(i => i.MenuCategory)
            .Where(i => i.MenuCategoryId == categoryId && i.DeletedAt == null && i.IsActive)
            .Select(i => new MenuItemDto
            {
                Id = i.Id,
                MenuCategoryId = i.MenuCategoryId,
                CategoryName = i.MenuCategory.CategoryName,
                ItemName = i.ItemName,
                ItemType = i.ItemType,
                PricePerPlate = i.PricePerPlate,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).OrderBy(i => i.ItemName).ToListAsync();
    }

    public async Task<MenuItemDto?> GetItemByIdAsync(int id)
    {
        return await _context.MenuItems
            .Include(i => i.MenuCategory)
            .Where(i => i.Id == id && i.DeletedAt == null)
            .Select(i => new MenuItemDto
            {
                Id = i.Id,
                MenuCategoryId = i.MenuCategoryId,
                CategoryName = i.MenuCategory.CategoryName,
                ItemName = i.ItemName,
                ItemType = i.ItemType,
                PricePerPlate = i.PricePerPlate,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<MenuItemDto> CreateItemAsync(CreateMenuItemDto dto)
    {
        var category = await _context.MenuCategories.FindAsync(dto.MenuCategoryId);
        if (category == null || category.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuCategory), dto.MenuCategoryId);

        var entity = new MMenuItem
        {
            MenuCategoryId = dto.MenuCategoryId,
            ItemName = dto.ItemName,
            ItemType = dto.ItemType,
            PricePerPlate = dto.PricePerPlate,
            IsActive = true
        };

        _context.MenuItems.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetItemByIdAsync(entity.Id))!;
    }

    public async Task<MenuItemDto> UpdateItemAsync(int id, UpdateMenuItemDto dto)
    {
        var entity = await _context.MenuItems.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuItem), id);

        entity.MenuCategoryId = dto.MenuCategoryId;
        entity.ItemName = dto.ItemName;
        entity.ItemType = dto.ItemType;
        entity.PricePerPlate = dto.PricePerPlate;

        await _context.SaveChangesAsync();
        return (await GetItemByIdAsync(id))!;
    }

    public async Task DeleteItemAsync(int id)
    {
        var entity = await _context.MenuItems.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuItem), id);

        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // === Packages ===

    public async Task<List<MenuPackageDto>> GetAllPackagesAsync(bool includeInactive = false)
    {
        var query = _context.MenuPackages
            .Include(p => p.PackageItems)
            .Where(p => p.DeletedAt == null);
        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query.Select(p => new MenuPackageDto
        {
            Id = p.Id,
            PackageName = p.PackageName,
            RatePerPlate = p.RatePerPlate,
            IsActive = p.IsActive,
            ItemCount = p.PackageItems.Count(pi => pi.DeletedAt == null),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).OrderBy(p => p.PackageName).ToListAsync();
    }

    public async Task<MenuPackageDetailDto?> GetPackageByIdAsync(int id)
    {
        var package = await _context.MenuPackages
            .Include(p => p.PackageItems.Where(pi => pi.DeletedAt == null))
                .ThenInclude(pi => pi.MenuItem)
                    .ThenInclude(i => i.MenuCategory)
            .Where(p => p.Id == id && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (package == null) return null;

        return new MenuPackageDetailDto
        {
            Id = package.Id,
            PackageName = package.PackageName,
            RatePerPlate = package.RatePerPlate,
            IsActive = package.IsActive,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt,
            Items = package.PackageItems.Select(pi => new MenuItemDto
            {
                Id = pi.MenuItem.Id,
                MenuCategoryId = pi.MenuItem.MenuCategoryId,
                CategoryName = pi.MenuItem.MenuCategory.CategoryName,
                ItemName = pi.MenuItem.ItemName,
                ItemType = pi.MenuItem.ItemType,
                PricePerPlate = pi.MenuItem.PricePerPlate,
                IsActive = pi.MenuItem.IsActive,
                CreatedAt = pi.MenuItem.CreatedAt,
                UpdatedAt = pi.MenuItem.UpdatedAt
            }).ToList()
        };
    }

    public async Task<MenuPackageDto> CreatePackageAsync(CreateMenuPackageDto dto)
    {
        var entity = new MMenuPackage
        {
            PackageName = dto.PackageName,
            RatePerPlate = dto.RatePerPlate,
            IsActive = true
        };

        _context.MenuPackages.Add(entity);
        await _context.SaveChangesAsync();

        // Add items
        foreach (var itemId in dto.MenuItemIds)
        {
            var item = await _context.MenuItems.FindAsync(itemId);
            if (item == null || item.DeletedAt != null)
                throw new NotFoundException(nameof(MMenuItem), itemId);

            _context.MenuPackageItems.Add(new MMenuPackageItem
            {
                MenuPackageId = entity.Id,
                MenuItemId = itemId
            });
        }

        await _context.SaveChangesAsync();

        return new MenuPackageDto
        {
            Id = entity.Id,
            PackageName = entity.PackageName,
            RatePerPlate = entity.RatePerPlate,
            IsActive = entity.IsActive,
            ItemCount = dto.MenuItemIds.Count,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<MenuPackageDto> UpdatePackageAsync(int id, UpdateMenuPackageDto dto)
    {
        var entity = await _context.MenuPackages
            .Include(p => p.PackageItems)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

        if (entity == null)
            throw new NotFoundException(nameof(MMenuPackage), id);

        entity.PackageName = dto.PackageName;
        entity.RatePerPlate = dto.RatePerPlate;

        // Remove existing items (soft delete)
        foreach (var existingItem in entity.PackageItems.Where(pi => pi.DeletedAt == null))
        {
            existingItem.DeletedAt = DateTime.UtcNow;
        }

        // Add new items
        foreach (var itemId in dto.MenuItemIds)
        {
            var item = await _context.MenuItems.FindAsync(itemId);
            if (item == null || item.DeletedAt != null)
                throw new NotFoundException(nameof(MMenuItem), itemId);

            _context.MenuPackageItems.Add(new MMenuPackageItem
            {
                MenuPackageId = entity.Id,
                MenuItemId = itemId
            });
        }

        await _context.SaveChangesAsync();

        return new MenuPackageDto
        {
            Id = entity.Id,
            PackageName = entity.PackageName,
            RatePerPlate = entity.RatePerPlate,
            IsActive = entity.IsActive,
            ItemCount = dto.MenuItemIds.Count,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task DeletePackageAsync(int id)
    {
        var entity = await _context.MenuPackages.FindAsync(id);
        if (entity == null || entity.DeletedAt != null)
            throw new NotFoundException(nameof(MMenuPackage), id);

        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
