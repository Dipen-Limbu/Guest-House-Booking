using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Hotel;

namespace Guest_House.Services.Hotel
{
    public interface IWebsiteSyncLogService
    {
        Task<List<WebsiteSyncLogResponseDto>> GetAllAsync(int? hotelId, string? syncStatus, string? entityType, CancellationToken cancellationToken = default);
        Task<WebsiteSyncLogResponseDto> GetByIdAsync(int syncId, CancellationToken cancellationToken = default);
        Task<WebsiteSyncLogResponseDto> CreateAsync(CreateWebsiteSyncLogDto dto, CancellationToken cancellationToken = default);
    }
}
