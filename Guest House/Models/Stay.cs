using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Stay
{
    public int StayId { get; set; }

    public int BookingId { get; set; }

    public DateTime? ActualCheckin { get; set; }

    public DateTime? ActualCheckout { get; set; }

    public string StayStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;


    public virtual ICollection<ExpenseCharge> ExpenseCharges { get; set; } = new List<ExpenseCharge>();

    public virtual ICollection<RoomOrder> RoomOrders { get; set; } = new List<RoomOrder>();
}
