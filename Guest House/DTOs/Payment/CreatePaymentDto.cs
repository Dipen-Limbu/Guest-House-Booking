using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Payment;

public class CreatePaymentDto
{
    [Required]
    public int BookingId { get; set; }

    public int? InvoiceId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = null!;

    [Required]
    public string PaymentType { get; set; } = null!;

    public string? TransactionRef { get; set; }
}