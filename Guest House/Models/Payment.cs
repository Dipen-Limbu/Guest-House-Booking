using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("payment")]
public partial class Payment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("booking_id")]
    public int BookingId { get; set; }

    [Column("invoice_id")]
    public int? InvoiceId { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("payment_method")]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = null!;

    [Column("payment_type")]
    [StringLength(20)]
    public string PaymentType { get; set; } = null!;

    [Column("payment_status")]
    [StringLength(20)]
    public string PaymentStatus { get; set; } = null!;

    [Column("transaction_ref")]
    [StringLength(100)]
    public string? TransactionRef { get; set; }

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Payments")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("InvoiceId")]
    [InverseProperty("Payments")]
    public virtual Invoice? Invoice { get; set; }
}
