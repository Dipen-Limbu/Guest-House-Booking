using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("invoice_item")]
public partial class InvoiceItem
{
    [Key]
    [Column("invoice_item_id")]
    public int InvoiceItemId { get; set; }

    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("item_type")]
    [StringLength(30)]
    public string ItemType { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string Description { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice Invoice { get; set; } = null!;
}
