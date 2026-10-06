using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Hotel;

namespace Guest_House.Services.Hotel
{
    public interface IHotelService
    {
        Task<List<HotelResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<HotelResponseDto> GetByIdAsync(int hotelId, CancellationToken cancellationToken = default);
        Task<HotelResponseDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default);
        Task<HotelResponseDto> UpdateAsync(int hotelId, UpdateHotelDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int hotelId, CancellationToken cancellationToken = default);
    }
}
