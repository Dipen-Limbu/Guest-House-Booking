using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class HotelExpense
{
    public int ExpenseId { get; set; }

    public int HotelId { get; set; }

    public string ExpenseCategory { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string? ReceiptUrl { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
}
