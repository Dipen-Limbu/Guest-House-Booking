using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Payment;

namespace Guest_House.Services.Payment
{
    public interface IPaymentService
    {
        Task<List<PaymentResponseDto>> GetAllAsync(int? bookingId, string? paymentStatus, string? paymentMethod, CancellationToken cancellationToken = default);
        Task<PaymentResponseDto> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default);
        Task<BookingPaymentSummaryDto> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default);
        Task<PaymentResponseDto> UpdateAsync(int paymentId, UpdatePaymentDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int paymentId, CancellationToken cancellationToken = default);
    }
}
