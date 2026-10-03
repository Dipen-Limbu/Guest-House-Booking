using Guest_House.Data;
using Guest_House.DTOs.Room;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Services.Room
{
    public class RoomService
    {
        public const string DefaultStatus = "available";
        public static readonly string[] AllowedStatuses = { "available", "occupied", "maintenance", "cleaning" };

        private readonly GuestHouseContext _context;
        public RoomService(GuestHouseContext context) => _context = context;

        public async Task<List<RoomResponseDto>> GetAllAsync(int? hotelId = null, int? categoryId = null, string? status = null)
        {
            var query = _context.Rooms.AsNoTracking()
                .Include(r => r.Category)
                .Include(r => r.RoomMedia)
                .AsQueryable();

            if (hotelId.HasValue) query = query.Where(r => r.HotelId == hotelId.Value);
            if (categoryId.HasValue) query = query.Where(r => r.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalized = NormalizeStatus(status);
                query = query.Where(r => r.Status == normalized);
            }

            var rooms = await query.OrderBy(r => r.HotelId).ThenBy(r => r.RoomNumber).ToListAsync();
            return rooms.Select(ToDto).ToList();
        }

        public async Task<RoomResponseDto> GetByIdAsync(int roomId)
        {
            var room = await _context.Rooms.AsNoTracking()
                .Include(r => r.Category).Include(r => r.RoomMedia)
                .FirstOrDefaultAsync(r => r.RoomId == roomId)
                ?? throw new KeyNotFoundException($"Room with id {roomId} was not found.");
            return ToDto(room);
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            if (!await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId))
                throw new KeyNotFoundException($"Hotel with id {dto.HotelId} was not found.");

            await EnsureCategoryBelongsToHotelAsync(dto.CategoryId, dto.HotelId);

            var roomNumber = dto.RoomNumber.Trim();
            if (await _context.Rooms.AnyAsync(r => r.HotelId == dto.HotelId && r.RoomNumber == roomNumber))
                throw new InvalidOperationException($"Room number '{roomNumber}' already exists in this hotel.");

            var now = DateTime.Now;
            var room = new Room
            {
                HotelId = dto.HotelId,
                CategoryId = dto.CategoryId,
                RoomNumber = roomNumber,
                FloorNumber = dto.FloorNumber,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? DefaultStatus : NormalizeStatus(dto.Status),
                Description = dto.Description?.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(room.RoomId);
        }

        public async Task<RoomResponseDto> UpdateAsync(int roomId, UpdateRoomDto dto)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId)
                ?? throw new KeyNotFoundException($"Room with id {roomId} was not found.");

            await EnsureCategoryBelongsToHotelAsync(dto.CategoryId, room.HotelId);

            var roomNumber = dto.RoomNumber.Trim();
            if (await _context.Rooms.AnyAsync(r =>
                    r.HotelId == room.HotelId && r.RoomNumber == roomNumber && r.RoomId != roomId))
                throw new InvalidOperationException($"Room number '{roomNumber}' already exists in this hotel.");

            room.CategoryId = dto.CategoryId;
            room.RoomNumber = roomNumber;
            room.FloorNumber = dto.FloorNumber;
            room.Description = dto.Description?.Trim();
            room.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(roomId);
        }

        public async Task<RoomResponseDto> UpdateStatusAsync(int roomId, UpdateRoomStatusDto dto)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId)
                ?? throw new KeyNotFoundException($"Room with id {roomId} was not found.");

            room.Status = NormalizeStatus(dto.Status);
            room.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(roomId);
        }

        public async Task DeleteAsync(int roomId)
        {
            var room = await _context.Rooms.Include(r => r.RoomMedia)
                .FirstOrDefaultAsync(r => r.RoomId == roomId)
                ?? throw new KeyNotFoundException($"Room with id {roomId} was not found.");

            var hasBookings = await _context.BookingRooms.AnyAsync(b => b.RoomId == roomId);
            var hasOrders = await _context.RoomOrders.AnyAsync(o => o.RoomId == roomId);
            if (hasBookings || hasOrders)
                throw new InvalidOperationException(
                    "Cannot delete a room that has booking or order history. Set its status to 'maintenance' instead.");

            _context.RoomMedia.RemoveRange(room.RoomMedia);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        // ---- helpers ----
        private async Task EnsureCategoryBelongsToHotelAsync(int categoryId, int hotelId)
        {
            var category = await _context.RoomCategories.AsNoTracking()
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new { c.HotelId })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Room category with id {categoryId} was not found.");

            if (category.HotelId != hotelId)
                throw new InvalidOperationException("The selected room category does not belong to this hotel.");
        }

        private static string NormalizeStatus(string status)
        {
            var value = status.Trim().ToLowerInvariant();
            if (!AllowedStatuses.Contains(value))
                throw new ArgumentException($"Invalid room status '{status}'. Allowed values: {string.Join(", ", AllowedStatuses)}.");
            return value;
        }

        private static RoomResponseDto ToDto(Room r) => new()
        {
            RoomId = r.RoomId,
            HotelId = r.HotelId,
            CategoryId = r.CategoryId,
            CategoryName = r.Category.CategoryName,
            BasePrice = r.Category.BasePrice,
            MaxOccupancy = r.Category.MaxOccupancy,
            RoomNumber = r.RoomNumber,
            FloorNumber = r.FloorNumber,
            Status = r.Status,
            Description = r.Description,
            Media = r.RoomMedia.OrderBy(m => m.DisplayOrder).ThenBy(m => m.MediaId)
                               .Select(RoomMediaService.ToDto).ToList(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
