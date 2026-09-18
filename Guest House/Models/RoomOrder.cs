using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("room_order")]
[Index("OrderNumber", Name = "UQ_room_order_number", IsUnique = true)]
public partial class RoomOrder
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("stay_id")]
    public int StayId { get; set; }

    [Column("room_id")]
    public int RoomId { get; set; }

    [Column("order_number")]
    [StringLength(50)]
    public string OrderNumber { get; set; } = null!;

    [Column("order_status")]
    [StringLength(20)]
    public string OrderStatus { get; set; } = null!;

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("ordered_at")]
    public DateTime? OrderedAt { get; set; }

    [InverseProperty("RoomOrder")]
    public virtual ICollection<ExpenseCharge> ExpenseCharges { get; set; } = new List<ExpenseCharge>();

    [ForeignKey("RoomId")]
    [InverseProperty("RoomOrders")]
    public virtual Room Room { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<RoomOrderItem> RoomOrderItems { get; set; } = new List<RoomOrderItem>();

    [ForeignKey("StayId")]
    [InverseProperty("RoomOrders")]
    public virtual Stay Stay { get; set; } = null!;
}
