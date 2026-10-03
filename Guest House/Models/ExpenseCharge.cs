using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class ExpenseCharge
{
    public int ChargeId { get; set; }

    public int StayId { get; set; }

    public string ChargeType { get; set; } = null!;

    public string? Description { get; set; }

    public int? RoomOrderId { get; set; }

    public decimal Amount { get; set; }

    public DateTime IncurredAt { get; set; }

    public virtual RoomOrder? RoomOrder { get; set; }

    public virtual Stay Stay { get; set; } = null!;
}
