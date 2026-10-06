using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Menu;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using MenuItemEntity = Guest_House.Models.MenuItem;

namespace Guest_House.Services.Menu
{
    public class MenuItemService : IMenuItemService
    {
        private readonly GuestHouseContext _context;

        public MenuItemService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItemResponseDto>> GetAllAsync(
            int? categoryId,
            bool? isAvailable,
            int? hotelId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.MenuItems
                .AsNoTracking()
                .Include(m => m.MenuCategory)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(m => m.MenuCategoryId == categoryId.Value);

            if (isAvailable.HasValue)
                query = query.Where(m => m.IsAvailable == isAvailable.Value);

            if (hotelId.HasValue)
                query = query.Where(m => m.MenuCategory.HotelId == hotelId.Value);

            var items = await query
                .OrderBy(m => m.ItemName)
                .ToListAsync(cancellationToken);

            return items.Select(ToDto).ToList();
        }

        public async Task<MenuItemResponseDto> GetByIdAsync(int menuItemId, CancellationToken cancellationToken = default)
        {
            var item = await _context.MenuItems
                .AsNoTracking()
                .Include(m => m.MenuCategory)
                .FirstOrDefaultAsync(m => m.MenuItemId == menuItemId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu item with ID {menuItemId} was not found.");

            return ToDto(item);
        }

        public async Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default)
        {
            var categoryExists = await _context.MenuCategories.AnyAsync(c => c.MenuCategoryId == dto.MenuCategoryId, cancellationToken);
            if (!categoryExists)
                throw new KeyNotFoundException($"Menu category with ID {dto.MenuCategoryId} was not found.");

            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            var itemName = dto.ItemName.Trim();
            var duplicate = await _context.MenuItems
                .AnyAsync(m => m.MenuCategoryId == dto.MenuCategoryId && m.ItemName.ToLower() == itemName.ToLower(), cancellationToken);

            if (duplicate)
                throw new InvalidOperationException($"Menu item '{itemName}' already exists in this category.");

            var now = DateTime.Now;
            var item = new MenuItemEntity
            {
                MenuCategoryId = dto.MenuCategoryId,
                ItemName = itemName,
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                ImageUrl = dto.ImageUrl?.Trim(),
                IsAvailable = dto.IsAvailable,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(item.MenuItemId, cancellationToken);
        }

        public async Task<MenuItemResponseDto> UpdateAsync(int menuItemId, UpdateMenuItemDto dto, CancellationToken cancellationToken = default)
        {
            var item = await _context.MenuItems
                .Include(m => m.MenuCategory)
                .FirstOrDefaultAsync(m => m.MenuItemId == menuItemId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu item with ID {menuItemId} was not found.");

            var categoryExists = await _context.MenuCategories.AnyAsync(c => c.MenuCategoryId == dto.MenuCategoryId, cancellationToken);
            if (!categoryExists)
                throw new KeyNotFoundException($"Menu category with ID {dto.MenuCategoryId} was not found.");

            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            var itemName = dto.ItemName.Trim();
            var duplicate = await _context.MenuItems
                .AnyAsync(m => m.MenuCategoryId == dto.MenuCategoryId && m.ItemName.ToLower() == itemName.ToLower() && m.MenuItemId != menuItemId, cancellationToken);

            if (duplicate)
                throw new InvalidOperationException($"Another menu item '{itemName}' already exists in this category.");

            item.MenuCategoryId = dto.MenuCategoryId;
            item.ItemName = itemName;
            item.Description = dto.Description?.Trim();
            item.Price = dto.Price;
            item.ImageUrl = dto.ImageUrl?.Trim();
            item.IsAvailable = dto.IsAvailable;
            item.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(menuItemId, cancellationToken);
        }

        public async Task DeleteAsync(int menuItemId, CancellationToken cancellationToken = default)
        {
            var item = await _context.MenuItems
                .Include(m => m.RoomOrderItems)
                .FirstOrDefaultAsync(m => m.MenuItemId == menuItemId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu item with ID {menuItemId} was not found.");

            if (item.RoomOrderItems.Any())
            {
                // Soft delete by setting unavailable if order history exists
                item.IsAvailable = false;
                item.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static MenuItemResponseDto ToDto(MenuItemEntity m)
        {
            return new MenuItemResponseDto
            {
                MenuItemId = m.MenuItemId,
                MenuCategoryId = m.MenuCategoryId,
                CategoryName = m.MenuCategory?.CategoryName ?? string.Empty,
                ItemName = m.ItemName,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                IsAvailable = m.IsAvailable,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            };
        }
    }
}
