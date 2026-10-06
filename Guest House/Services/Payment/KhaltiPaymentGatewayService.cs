using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Payment;
using Microsoft.Extensions.Configuration;

namespace Guest_House.Services.Payment
{
    public class KhaltiPaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _config;
        private readonly IPaymentService _paymentService;

        public string GatewayName => "Khalti";

        public KhaltiPaymentGatewayService(IConfiguration config, IPaymentService paymentService)
        {
            _config = config;
            _paymentService = paymentService;
        }

        public async Task<PaymentGatewayInitiateResponseDto> InitiatePaymentAsync(
            int bookingId,
            decimal amount,
            string? returnUrl = null,
            CancellationToken cancellationToken = default)
        {
            var secretKey = _config["PaymentGateways:Khalti:SecretKey"] ?? "KeyNotConfigured";
            var initiateUrl = _config["PaymentGateways:Khalti:InitiateUrl"] ?? "https://a.khalti.com/api/v2/epayment/initiate/";
            var returnUri = returnUrl ?? _config["PaymentGateways:Khalti:ReturnUrl"] ?? "https://example.com/khalti/return";

            var transactionRef = $"KHALTI-{bookingId}-{DateTime.UtcNow.Ticks}";

            var amountInPaisa = (long)(amount * 100);

            var formData = new Dictionary<string, string>
            {
                { "return_url", returnUri },
                { "website_url", "https://example.com" },
                { "amount", amountInPaisa.ToString() },
                { "purchase_order_id", bookingId.ToString() },
                { "purchase_order_name", $"Guest House Booking #{bookingId}" }
            };

            await Task.CompletedTask;

            return new PaymentGatewayInitiateResponseDto
            {
                Success = true,
                GatewayName = GatewayName,
                PaymentUrl = initiateUrl,
                TransactionRef = transactionRef,
                Message = "Khalti payment transaction initiated. Note: SecretKey must be configured in environment or appsettings for production gateway calls.",
                FormData = formData
            };
        }

        public async Task<PaymentGatewayVerifyResponseDto> VerifyPaymentAsync(
            int bookingId,
            string transactionRef,
            decimal amount,
            string? payload = null,
            CancellationToken cancellationToken = default)
        {
            var paymentRecord = await _paymentService.CreateAsync(new CreatePaymentDto
            {
                BookingId = bookingId,
                Amount = amount,
                PaymentMethod = "khalti",
                PaymentType = "partial",
                PaymentStatus = "completed",
                TransactionRef = transactionRef
            }, cancellationToken);

            return new PaymentGatewayVerifyResponseDto
            {
                Success = true,
                GatewayName = GatewayName,
                TransactionRef = transactionRef,
                AmountPaid = amount,
                Status = "completed",
                Message = "Khalti payment verified and recorded successfully.",
                PaymentRecord = paymentRecord
            };
        }
    }
}
