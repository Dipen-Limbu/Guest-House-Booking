using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class InvoiceItem
{
    public int InvoiceItemId { get; set; }

    public int InvoiceId { get; set; }

    public string ItemType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Amount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
