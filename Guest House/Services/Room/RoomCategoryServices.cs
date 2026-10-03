using Guest_House.Data;
using Guest_House.DTOs.Room;
using Guest_House.Models;

namespace Guest_House.Services.Room
{
    public class RoomCategoryService
    {
        private readonly GuestHouseContext _context;
        public RoomCategoryService(GuestHouseContext context) => _context = context;

        public async Task<List<RoomCategoryResponseDto>> GetAllAsync(int? hotelId = null)
        {
            var query = _context.RoomCategories.AsNoTracking().AsQueryable();
            if (hotelId.HasValue)
                query = query.Where(c => c.HotelId == hotelId.Value);

            return await query
                .OrderBy(c => c.BasePrice)
                .Select(c => new RoomCategoryResponseDto
                {
                    CategoryId = c.CategoryId,
                    HotelId = c.HotelId,
                    CategoryName = c.CategoryName,
                    BasePrice = c.BasePrice,
                    MaxOccupancy = c.MaxOccupancy,
                    Description = c.Description,
                    RoomCount = c.Rooms.Count,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<RoomCategoryResponseDto> GetByIdAsync(int categoryId)
        {
            var dto = await _context.RoomCategories.AsNoTracking()
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new RoomCategoryResponseDto { /* same projection as above */ })
                .FirstOrDefaultAsync();

            return dto ?? throw new KeyNotFoundException($"Room category with id {categoryId} was not found.");
        }

        public async Task<RoomCategoryResponseDto> CreateAsync(CreateRoomCategoryDto dto)
        {
            if (!await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId))
                throw new KeyNotFoundException($"Hotel with id {dto.HotelId} was not found.");

            var name = dto.CategoryName.Trim();
            if (await _context.RoomCategories.AnyAsync(c => c.HotelId == dto.HotelId && c.CategoryName == name))
                throw new InvalidOperationException($"A room category named '{name}' already exists for this hotel.");

            var now = DateTime.Now;
            var entity = new RoomCategory
            {
                HotelId = dto.HotelId,
                CategoryName = name,
                BasePrice = dto.BasePrice,
                MaxOccupancy = dto.MaxOccupancy,
                Description = dto.Description?.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.RoomCategories.Add(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.CategoryId);
        }

        public async Task<RoomCategoryResponseDto> UpdateAsync(int categoryId, UpdateRoomCategoryDto dto)
        {
            var entity = await _context.RoomCategories.FirstOrDefaultAsync(c => c.CategoryId == categoryId)
                ?? throw new KeyNotFoundException($"Room category with id {categoryId} was not found.");

            var name = dto.CategoryName.Trim();
            if (await _context.RoomCategories.AnyAsync(c =>
                    c.HotelId == entity.HotelId && c.CategoryName == name && c.CategoryId != categoryId))
                throw new InvalidOperationException($"A room category named '{name}' already exists for this hotel.");

            entity.CategoryName = name;
            entity.BasePrice = dto.BasePrice;
            entity.MaxOccupancy = dto.MaxOccupancy;
            entity.Description = dto.Description?.Trim();
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(categoryId);
        }

        public async Task DeleteAsync(int categoryId)
        {
            var entity = await _context.RoomCategories.FirstOrDefaultAsync(c => c.CategoryId == categoryId)
                ?? throw new KeyNotFoundException($"Room category with id {categoryId} was not found.");

            if (await _context.Rooms.AnyAsync(r => r.CategoryId == categoryId))
                throw new InvalidOperationException("Cannot delete a room category that still has rooms assigned to it.");

            _context.RoomCategories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
