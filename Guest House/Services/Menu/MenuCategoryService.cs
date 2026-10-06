using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Menu;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using MenuCategoryEntity = Guest_House.Models.MenuCategory;

namespace Guest_House.Services.Menu
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly GuestHouseContext _context;

        public MenuCategoryService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<MenuCategoryResponseDto>> GetAllAsync(int? hotelId, CancellationToken cancellationToken = default)
        {
            var query = _context.MenuCategories
                .AsNoTracking()
                .Include(c => c.MenuItems)
                .AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(c => c.HotelId == hotelId.Value);

            var categories = await query
                .OrderBy(c => c.CategoryName)
                .ToListAsync(cancellationToken);

            return categories.Select(ToDto).ToList();
        }

        public async Task<MenuCategoryResponseDto> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _context.MenuCategories
                .AsNoTracking()
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.MenuCategoryId == categoryId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu category with ID {categoryId} was not found.");

            return ToDto(category);
        }

        public async Task<MenuCategoryResponseDto> CreateAsync(CreateMenuCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId, cancellationToken);
            if (!hotelExists)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} was not found.");

            var categoryName = dto.CategoryName.Trim();
            var duplicate = await _context.MenuCategories
                .AnyAsync(c => c.HotelId == dto.HotelId && c.CategoryName.ToLower() == categoryName.ToLower(), cancellationToken);

            if (duplicate)
                throw new InvalidOperationException($"Menu category '{categoryName}' already exists for this hotel.");

            var category = new MenuCategoryEntity
            {
                HotelId = dto.HotelId,
                CategoryName = categoryName,
                Description = dto.Description?.Trim()
            };

            _context.MenuCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(category.MenuCategoryId, cancellationToken);
        }

        public async Task<MenuCategoryResponseDto> UpdateAsync(int categoryId, UpdateMenuCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _context.MenuCategories
                .FirstOrDefaultAsync(c => c.MenuCategoryId == categoryId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu category with ID {categoryId} was not found.");

            var categoryName = dto.CategoryName.Trim();
            var duplicate = await _context.MenuCategories
                .AnyAsync(c => c.HotelId == category.HotelId && c.CategoryName.ToLower() == categoryName.ToLower() && c.MenuCategoryId != categoryId, cancellationToken);

            if (duplicate)
                throw new InvalidOperationException($"Another menu category '{categoryName}' already exists for this hotel.");

            category.CategoryName = categoryName;
            category.Description = dto.Description?.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(categoryId, cancellationToken);
        }

        public async Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.MenuCategoryId == categoryId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu category with ID {categoryId} was not found.");

            if (category.MenuItems.Any())
                throw new InvalidOperationException("Cannot delete menu category that contains active menu items. Please reassign or delete menu items first.");

            _context.MenuCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static MenuCategoryResponseDto ToDto(MenuCategoryEntity c)
        {
            return new MenuCategoryResponseDto
            {
                MenuCategoryId = c.MenuCategoryId,
                HotelId = c.HotelId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ItemCount = c.MenuItems?.Count ?? 0
            };
        }
    }
}
