using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Stay;

namespace Guest_House.Services.Stay
{
    public interface IStayService
    {
        Task<StayResponseDto> CheckInAsync(CheckInDto dto, CancellationToken cancellationToken = default);
        Task<StayResponseDto> CheckOutAsync(int stayId, CancellationToken cancellationToken = default);
        Task<List<StayResponseDto>> GetAllAsync(int? bookingId, string? stayStatus, CancellationToken cancellationToken = default);
        Task<StayResponseDto> GetByIdAsync(int stayId, CancellationToken cancellationToken = default);
        Task<List<StayResponseDto>> GetActiveStaysAsync(CancellationToken cancellationToken = default);
        Task<StayResponseDto> UpdateAsync(int stayId, UpdateStayDto dto, CancellationToken cancellationToken = default);
    }
}
