using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Hotel;

namespace Guest_House.Services.Hotel
{
    public interface IHotelExpenseService
    {
        Task<List<HotelExpenseResponseDto>> GetAllAsync(int? hotelId, string? category, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
        Task<HotelExpenseResponseDto> GetByIdAsync(int expenseId, CancellationToken cancellationToken = default);
        Task<HotelExpenseSummaryDto> GetSummaryAsync(int hotelId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
        Task<HotelExpenseResponseDto> CreateAsync(CreateHotelExpenseDto dto, CancellationToken cancellationToken = default);
        Task<HotelExpenseResponseDto> UpdateAsync(int expenseId, UpdateHotelExpenseDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int expenseId, CancellationToken cancellationToken = default);
    }
}
