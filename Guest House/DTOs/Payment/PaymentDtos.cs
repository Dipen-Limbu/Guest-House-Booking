using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        public int? InvoiceId { get; set; }

        [Required(ErrorMessage = "Payment amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string PaymentMethod { get; set; } = null!;

        [Required(ErrorMessage = "Payment type is required.")]
        [StringLength(50, ErrorMessage = "Payment type cannot exceed 50 characters.")]
        public string PaymentType { get; set; } = null!;

        public string? PaymentStatus { get; set; }

        [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionRef { get; set; }
    }

    public class UpdatePaymentDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal? Amount { get; set; }

        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }

        [StringLength(50, ErrorMessage = "Payment type cannot exceed 50 characters.")]
        public string? PaymentType { get; set; }

        [StringLength(50, ErrorMessage = "Payment status cannot exceed 50 characters.")]
        public string? PaymentStatus { get; set; }

        [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionRef { get; set; }
    }

    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int? InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentType { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string? TransactionRef { get; set; }
        public DateTime PaidAt { get; set; }
        public string? BookingReference { get; set; }
        public decimal? TotalBookingAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; }
        public decimal? RemainingDueAmount { get; set; }
    }

    public class BookingPaymentSummaryDto
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = null!;
        public decimal TotalBookingAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal RemainingDueAmount { get; set; }
        public List<PaymentResponseDto> Payments { get; set; } = new();
    }

    public class EsewaInitiateRequestDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string SuccessUrl { get; set; } = "https://example.com/esewa/success";
        public string FailureUrl { get; set; } = "https://example.com/esewa/failure";
    }

    public class EsewaVerifyRequestDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string TransactionRef { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        public string? EncodedResponse { get; set; }
    }

    public class KhaltiInitiateRequestDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string ReturnUrl { get; set; } = "https://example.com/khalti/return";
        public string WebsiteUrl { get; set; } = "https://example.com";
    }

    public class KhaltiVerifyRequestDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public string pidx { get; set; } = null!;

        public string? TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }

    public class PaymentGatewayInitiateResponseDto
    {
        public bool Success { get; set; }
        public string GatewayName { get; set; } = null!;
        public string? PaymentUrl { get; set; }
        public string? PaymentToken { get; set; }
        public string? TransactionRef { get; set; }
        public string Message { get; set; } = null!;
        public IDictionary<string, string>? FormData { get; set; }
    }

    public class PaymentGatewayVerifyResponseDto
    {
        public bool Success { get; set; }
        public string GatewayName { get; set; } = null!;
        public string TransactionRef { get; set; } = null!;
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public PaymentResponseDto? PaymentRecord { get; set; }
    }
}
