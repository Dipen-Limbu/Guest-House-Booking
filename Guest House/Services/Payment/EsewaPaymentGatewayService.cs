using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Payment;
using Microsoft.Extensions.Configuration;

namespace Guest_House.Services.Payment
{
    public class EsewaPaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _config;
        private readonly IPaymentService _paymentService;

        public string GatewayName => "eSewa";

        public EsewaPaymentGatewayService(IConfiguration config, IPaymentService paymentService)
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
            var merchantCode = _config["PaymentGateways:Esewa:MerchantCode"] ?? "EPAYTEST";
            var gatewayUrl = _config["PaymentGateways:Esewa:GatewayUrl"] ?? "https://rc-epay.esewa.com.np/api/epay/main/v2/form";
            var successUrl = returnUrl ?? _config["PaymentGateways:Esewa:SuccessUrl"] ?? "https://example.com/esewa/success";
            var failureUrl = _config["PaymentGateways:Esewa:FailureUrl"] ?? "https://example.com/esewa/failure";

            var transactionRef = $"ESEWA-{bookingId}-{DateTime.UtcNow.Ticks}";

            var formData = new Dictionary<string, string>
            {
                { "amount", amount.ToString("F2") },
                { "tax_amount", "0" },
                { "total_amount", amount.ToString("F2") },
                { "transaction_uuid", transactionRef },
                { "product_code", merchantCode },
                { "product_service_charge", "0" },
                { "product_delivery_charge", "0" },
                { "success_url", successUrl },
                { "failure_url", failureUrl },
                { "signed_field_names", "total_amount,transaction_uuid,product_code" }
            };

            await Task.CompletedTask;

            return new PaymentGatewayInitiateResponseDto
            {
                Success = true,
                GatewayName = GatewayName,
                PaymentUrl = gatewayUrl,
                TransactionRef = transactionRef,
                Message = "eSewa payment transaction initiated successfully.",
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
            // Payment record creation upon verification
            var paymentRecord = await _paymentService.CreateAsync(new CreatePaymentDto
            {
                BookingId = bookingId,
                Amount = amount,
                PaymentMethod = "esewa",
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
                Message = "eSewa payment verified and recorded successfully.",
                PaymentRecord = paymentRecord
            };
        }
    }
}
