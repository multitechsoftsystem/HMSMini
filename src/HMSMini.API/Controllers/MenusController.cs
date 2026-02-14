using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HMSMini.API.Models.DTOs.MenuCategory;
using HMSMini.API.Models.DTOs.MenuItem;
using HMSMini.API.Models.DTOs.MenuPackage;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Controllers;

[ApiController]
[Route("api/menus")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenusController> _logger;

    public MenusController(IMenuService menuService, ILogger<MenusController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    // === Categories ===

    [HttpGet("categories")]
    public async Task<ActionResult<List<MenuCategoryDto>>> GetAllCategories([FromQuery] bool includeInactive = false)
    {
        var categories = await _menuService.GetAllCategoriesAsync(includeInactive);
        return Ok(categories);
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<MenuCategoryDto>> GetCategoryById(int id)
    {
        var category = await _menuService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound($"Menu category with ID {id} not found.");
        return Ok(category);
    }

    [HttpPost("categories")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuCategoryDto>> CreateCategory([FromBody] CreateMenuCategoryDto dto)
    {
        var category = await _menuService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [HttpPut("categories/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuCategoryDto>> UpdateCategory(int id, [FromBody] UpdateMenuCategoryDto dto)
    {
        var category = await _menuService.UpdateCategoryAsync(id, dto);
        return Ok(category);
    }

    [HttpDelete("categories/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        await _menuService.DeleteCategoryAsync(id);
        return NoContent();
    }

    // === Items ===

    [HttpGet("items")]
    public async Task<ActionResult<List<MenuItemDto>>> GetAllItems([FromQuery] bool includeInactive = false)
    {
        var items = await _menuService.GetAllItemsAsync(includeInactive);
        return Ok(items);
    }

    [HttpGet("items/category/{categoryId}")]
    public async Task<ActionResult<List<MenuItemDto>>> GetItemsByCategory(int categoryId)
    {
        var items = await _menuService.GetItemsByCategoryAsync(categoryId);
        return Ok(items);
    }

    [HttpGet("items/{id}")]
    public async Task<ActionResult<MenuItemDto>> GetItemById(int id)
    {
        var item = await _menuService.GetItemByIdAsync(id);
        if (item == null) return NotFound($"Menu item with ID {id} not found.");
        return Ok(item);
    }

    [HttpPost("items")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuItemDto>> CreateItem([FromBody] CreateMenuItemDto dto)
    {
        var item = await _menuService.CreateItemAsync(dto);
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [HttpPut("items/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuItemDto>> UpdateItem(int id, [FromBody] UpdateMenuItemDto dto)
    {
        var item = await _menuService.UpdateItemAsync(id, dto);
        return Ok(item);
    }

    [HttpDelete("items/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteItem(int id)
    {
        await _menuService.DeleteItemAsync(id);
        return NoContent();
    }

    // === Packages ===

    [HttpGet("packages")]
    public async Task<ActionResult<List<MenuPackageDto>>> GetAllPackages([FromQuery] bool includeInactive = false)
    {
        var packages = await _menuService.GetAllPackagesAsync(includeInactive);
        return Ok(packages);
    }

    [HttpGet("packages/{id}")]
    public async Task<ActionResult<MenuPackageDetailDto>> GetPackageById(int id)
    {
        var package = await _menuService.GetPackageByIdAsync(id);
        if (package == null) return NotFound($"Menu package with ID {id} not found.");
        return Ok(package);
    }

    [HttpPost("packages")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuPackageDto>> CreatePackage([FromBody] CreateMenuPackageDto dto)
    {
        var package = await _menuService.CreatePackageAsync(dto);
        return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
    }

    [HttpPut("packages/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MenuPackageDto>> UpdatePackage(int id, [FromBody] UpdateMenuPackageDto dto)
    {
        var package = await _menuService.UpdatePackageAsync(id, dto);
        return Ok(package);
    }

    [HttpDelete("packages/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePackage(int id)
    {
        await _menuService.DeletePackageAsync(id);
        return NoContent();
    }
}
