using Guest_House.DTOs.Payment;

namespace Guest_House.Services;

public interface IPaymentService
{

    Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto);

    Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId);

    Task<List<PaymentResponseDto>> GetPaymentsByBookingAsync(int bookingId);
    Task<bool> UpdatePaymentStatusAsync(int paymentId, string status);
}