using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("invoice")]
[Index("BookingId", Name = "UQ_invoice_booking", IsUnique = true)]
[Index("InvoiceNumber", Name = "UQ_invoice_number", IsUnique = true)]
public partial class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("booking_id")]
    public int BookingId { get; set; }

    [Column("invoice_number")]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = null!;

    [Column("room_charge_total", TypeName = "decimal(10, 2)")]
    public decimal RoomChargeTotal { get; set; }

    [Column("extra_charge_total", TypeName = "decimal(10, 2)")]
    public decimal ExtraChargeTotal { get; set; }

    [Column("tax_amount", TypeName = "decimal(10, 2)")]
    public decimal TaxAmount { get; set; }

    [Column("discount_amount", TypeName = "decimal(10, 2)")]
    public decimal DiscountAmount { get; set; }

    [Column("grand_total", TypeName = "decimal(10, 2)")]
    public decimal GrandTotal { get; set; }

    [Column("paid_amount", TypeName = "decimal(10, 2)")]
    public decimal PaidAmount { get; set; }

    [Column("due_amount", TypeName = "decimal(10, 2)")]
    public decimal DueAmount { get; set; }

    [Column("invoice_status")]
    [StringLength(20)]
    public string InvoiceStatus { get; set; } = null!;

    [Column("generated_at")]
    public DateTime? GeneratedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Invoice")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("Invoice")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
