using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Payment
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

    public virtual Booking Booking { get; set; } = null!;

    public virtual Invoice? Invoice { get; set; }
}
