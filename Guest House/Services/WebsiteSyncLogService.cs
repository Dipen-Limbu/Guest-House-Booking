using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Guest_House.Data;
using Guest_House.Models;
using Guest_House.DTOs.WebsiteSyncLog;
using System;

namespace Guest_House.Services
{
    public class WebsiteSyncLogService
    {
        private readonly GuestHouseContext _context;

        public WebsiteSyncLogService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WebsiteSyncLogDto>> GetAllLogsAsync()
        {
            return await _context.WebsiteSyncLogs
                .AsNoTracking()
                .Select(l => new WebsiteSyncLogDto
                {
                    SyncId = l.SyncId,
                    HotelId = l.HotelId,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    SyncStatus = l.SyncStatus,
                    ErrorMessage = l.ErrorMessage,
                    SyncedAt = l.SyncedAt
                })
                .ToListAsync();
        }

        public async Task<WebsiteSyncLogDto?> GetLogByIdAsync(int syncId)
        {
            return await _context.WebsiteSyncLogs
                .AsNoTracking()
                .Where(l => l.SyncId == syncId)
                .Select(l => new WebsiteSyncLogDto
                {
                    SyncId = l.SyncId,
                    HotelId = l.HotelId,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    SyncStatus = l.SyncStatus,
                    ErrorMessage = l.ErrorMessage,
                    SyncedAt = l.SyncedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WebsiteSyncLogDto>> GetLogsByHotelIdAsync(int hotelId, string? entityType = null, string? syncStatus = null)
        {
            var query = _context.WebsiteSyncLogs.AsNoTracking().Where(l => l.HotelId == hotelId);

            if (!string.IsNullOrEmpty(entityType))
            {
                query = query.Where(l => l.EntityType == entityType);
            }
            if (!string.IsNullOrEmpty(syncStatus))
            {
                query = query.Where(l => l.SyncStatus == syncStatus);
            }

            return await query
                .Select(l => new WebsiteSyncLogDto
                {
                    SyncId = l.SyncId,
                    HotelId = l.HotelId,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    SyncStatus = l.SyncStatus,
                    ErrorMessage = l.ErrorMessage,
                    SyncedAt = l.SyncedAt
                })
                .ToListAsync();
        }

        public async Task<WebsiteSyncLogDto?> CreateLogAsync(WebsiteSyncLogCreateDto dto)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId);
            if (!hotelExists) return null;

            var log = new WebsiteSyncLog
            {
                HotelId = dto.HotelId,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                SyncStatus = dto.SyncStatus,
                ErrorMessage = dto.SyncStatus == "success" ? null : dto.ErrorMessage,
                SyncedAt = dto.SyncedAt ?? DateTime.Now
            };

            _context.WebsiteSyncLogs.Add(log);
            await _context.SaveChangesAsync();

            return new WebsiteSyncLogDto
            {
                SyncId = log.SyncId,
                HotelId = log.HotelId,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                SyncStatus = log.SyncStatus,
                ErrorMessage = log.ErrorMessage,
                SyncedAt = log.SyncedAt
            };
        }
    }
}
