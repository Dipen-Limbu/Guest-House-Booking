using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("hotel_expense")]
public partial class HotelExpense
{
    [Key]
    [Column("expense_id")]
    public int ExpenseId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("expense_category")]
    [StringLength(100)]
    public string ExpenseCategory { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("payment_method")]
    [StringLength(30)]
    public string? PaymentMethod { get; set; }

    [Column("expense_date")]
    public DateTime? ExpenseDate { get; set; }

    [Column("receipt_url")]
    [StringLength(500)]
    public string? ReceiptUrl { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("HotelExpenses")]
    public virtual Hotel Hotel { get; set; } = null!;
}
