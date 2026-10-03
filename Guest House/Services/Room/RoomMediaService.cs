using Guest_House.Data;
using Guest_House.DTOs.Room;
using Guest_House.DTOs.RoomMedia;
using Microsoft.EntityFrameworkCore;
using RoomMediumEntity = global::Guest_House.Models.RoomMedium;

namespace Guest_House.Services
{
    public class RoomMediaService
    {
        public static readonly string[] AllowedMediaTypes = { "image", "video" };

        private readonly GuestHouseContext _context;

        public RoomMediaService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<RoomMediaResponseDto>> GetByRoomAsync(int roomId)
        {
            await EnsureRoomExistsAsync(roomId);

            var items = await _context.RoomMedia
                .AsNoTracking()
                .Where(m => m.RoomId == roomId)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.MediaId)
                .ToListAsync();

            return items.Select(m => ToDto(m)).ToList();
        }

        public async Task<RoomMediaResponseDto> GetByIdAsync(int mediaId)
        {
            var entity = await _context.RoomMedia.AsNoTracking().FirstOrDefaultAsync(m => m.MediaId == mediaId)
                ?? throw new KeyNotFoundException($"Room media with id {mediaId} was not found.");

            return ToDto(entity);
        }

        public async Task<RoomMediaResponseDto> CreateAsync(int roomId, CreateRoomMediaDto dto)
        {
            await EnsureRoomExistsAsync(roomId);

            var entity = new RoomMediumEntity
            {
                RoomId = roomId,
                MediaType = NormalizeMediaType(dto.MediaType),
                FileUrl = dto.FileUrl.Trim(),
                Caption = dto.Caption?.Trim(),
                DisplayOrder = dto.DisplayOrder,
                UploadedAt = DateTime.Now
            };

            _context.RoomMedia.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<RoomMediaResponseDto> UpdateAsync(int mediaId, UpdateRoomMediaDto dto)
        {
            var entity = await _context.RoomMedia.FirstOrDefaultAsync(m => m.MediaId == mediaId)
                ?? throw new KeyNotFoundException($"Room media with id {mediaId} was not found.");

            entity.MediaType = NormalizeMediaType(dto.MediaType);
            entity.FileUrl = dto.FileUrl.Trim();
            entity.Caption = dto.Caption?.Trim();
            entity.DisplayOrder = dto.DisplayOrder;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task DeleteAsync(int mediaId)
        {
            var entity = await _context.RoomMedia.FirstOrDefaultAsync(m => m.MediaId == mediaId)
                ?? throw new KeyNotFoundException($"Room media with id {mediaId} was not found.");

            _context.RoomMedia.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // ---- helpers ----

        private async Task EnsureRoomExistsAsync(int roomId)
        {
            var exists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
            if (!exists)
                throw new KeyNotFoundException($"Room with id {roomId} was not found.");
        }

        private static string NormalizeMediaType(string mediaType)
        {
            var value = mediaType.Trim().ToLowerInvariant();
            if (!AllowedMediaTypes.Contains(value))
                throw new ArgumentException($"Invalid media type '{mediaType}'. Allowed values: {string.Join(", ", AllowedMediaTypes)}.");
            return value;
        }

        public static RoomMediaResponseDto ToDto(RoomMediumEntity m)
        {
            return new RoomMediaResponseDto
            {
                MediaId = m.MediaId,
                RoomId = m.RoomId,
                MediaType = m.MediaType,
                FileUrl = m.FileUrl,
                Caption = m.Caption,
                DisplayOrder = Convert.ToInt32(m.DisplayOrder), // works for int and int?
                UploadedAt = m.UploadedAt
            };
        }
    }
}