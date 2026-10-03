using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class RoomOrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string? SpecialInstruction { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual RoomOrder Order { get; set; } = null!;
}
