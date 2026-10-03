using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int BookingId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public decimal RoomChargeTotal { get; set; }

    public decimal ExtraChargeTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public string InvoiceStatus { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
