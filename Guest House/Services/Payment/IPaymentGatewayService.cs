using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Payment;

namespace Guest_House.Services.Payment
{
    public interface IPaymentGatewayService
    {
        string GatewayName { get; }
        Task<PaymentGatewayInitiateResponseDto> InitiatePaymentAsync(int bookingId, decimal amount, string? returnUrl = null, CancellationToken cancellationToken = default);
        Task<PaymentGatewayVerifyResponseDto> VerifyPaymentAsync(int bookingId, string transactionRef, decimal amount, string? payload = null, CancellationToken cancellationToken = default);
    }
}
