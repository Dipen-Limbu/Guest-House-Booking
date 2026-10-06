using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Hotel;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using WebsiteSyncLogEntity = Guest_House.Models.WebsiteSyncLog;

namespace Guest_House.Services.Hotel
{
    public class WebsiteSyncLogService : IWebsiteSyncLogService
    {
        public static readonly string[] AllowedEntityTypes = { "room", "category", "availability", "price" };
        public static readonly string[] AllowedSyncStatuses = { "success", "failed" };

        private readonly GuestHouseContext _context;

        public WebsiteSyncLogService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<WebsiteSyncLogResponseDto>> GetAllAsync(
            int? hotelId,
            string? syncStatus,
            string? entityType,
            CancellationToken cancellationToken = default)
        {
            var query = _context.WebsiteSyncLogs
                .AsNoTracking()
                .Include(s => s.Hotel)
                .AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(s => s.HotelId == hotelId.Value);

            if (!string.IsNullOrWhiteSpace(syncStatus))
                query = query.Where(s => s.SyncStatus.ToLower() == syncStatus.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(s => s.EntityType.ToLower() == entityType.Trim().ToLower());

            var logs = await query
                .OrderByDescending(s => s.SyncedAt)
                .ToListAsync(cancellationToken);

            return logs.Select(ToDto).ToList();
        }

        public async Task<WebsiteSyncLogResponseDto> GetByIdAsync(int syncId, CancellationToken cancellationToken = default)
        {
            var log = await _context.WebsiteSyncLogs
                .AsNoTracking()
                .Include(s => s.Hotel)
                .FirstOrDefaultAsync(s => s.SyncId == syncId, cancellationToken)
                ?? throw new KeyNotFoundException($"Website sync log record with ID {syncId} was not found.");

            return ToDto(log);
        }

        public async Task<WebsiteSyncLogResponseDto> CreateAsync(CreateWebsiteSyncLogDto dto, CancellationToken cancellationToken = default)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId, cancellationToken);
            if (!hotelExists)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} was not found.");

            var entityType = dto.EntityType.Trim().ToLower();
            if (!AllowedEntityTypes.Contains(entityType))
                throw new ArgumentException($"Invalid entity type '{dto.EntityType}'. Allowed values: {string.Join(", ", AllowedEntityTypes)}.");

            var syncStatus = dto.SyncStatus.Trim().ToLower();
            if (!AllowedSyncStatuses.Contains(syncStatus))
                throw new ArgumentException($"Invalid sync status '{dto.SyncStatus}'. Allowed values: {string.Join(", ", AllowedSyncStatuses)}.");

            var log = new WebsiteSyncLogEntity
            {
                HotelId = dto.HotelId,
                EntityType = entityType,
                EntityId = dto.EntityId,
                SyncStatus = syncStatus,
                ErrorMessage = dto.ErrorMessage?.Trim(),
                SyncedAt = DateTime.Now
            };

            _context.WebsiteSyncLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(log.SyncId, cancellationToken);
        }

        private static WebsiteSyncLogResponseDto ToDto(WebsiteSyncLogEntity s)
        {
            return new WebsiteSyncLogResponseDto
            {
                SyncId = s.SyncId,
                HotelId = s.HotelId,
                HotelName = s.Hotel?.Name ?? string.Empty,
                EntityType = s.EntityType,
                EntityId = s.EntityId,
                SyncStatus = s.SyncStatus,
                ErrorMessage = s.ErrorMessage,
                SyncedAt = s.SyncedAt
            };
        }
    }
}
