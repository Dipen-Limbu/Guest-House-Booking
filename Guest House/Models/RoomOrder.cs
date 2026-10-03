using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class RoomOrder
{
    public int OrderId { get; set; }

    public int StayId { get; set; }

    public int RoomId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime OrderedAt { get; set; }

    public virtual ICollection<ExpenseCharge> ExpenseCharges { get; set; } = new List<ExpenseCharge>();

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<RoomOrderItem> RoomOrderItems { get; set; } = new List<RoomOrderItem>();

    public virtual Stay Stay { get; set; } = null!;
}
