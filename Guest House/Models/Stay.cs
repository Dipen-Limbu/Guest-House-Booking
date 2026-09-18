using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("stay")]
[Index("BookingId", Name = "UQ_stay_booking", IsUnique = true)]
public partial class Stay
{
    [Key]
    [Column("stay_id")]
    public int StayId { get; set; }

    [Column("booking_id")]
    public int BookingId { get; set; }

    [Column("actual_checkin")]
    public DateTime? ActualCheckin { get; set; }

    [Column("actual_checkout")]
    public DateTime? ActualCheckout { get; set; }

    [Column("stay_status")]
    [StringLength(20)]
    public string StayStatus { get; set; } = null!;

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Stay")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Stay")]
    public virtual ICollection<ExpenseCharge> ExpenseCharges { get; set; } = new List<ExpenseCharge>();

    [InverseProperty("Stay")]
    public virtual ICollection<RoomOrder> RoomOrders { get; set; } = new List<RoomOrder>();
}
