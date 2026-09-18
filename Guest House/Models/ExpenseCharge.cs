using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("expense_charge")]
public partial class ExpenseCharge
{
    [Key]
    [Column("charge_id")]
    public int ChargeId { get; set; }

    [Column("stay_id")]
    public int StayId { get; set; }

    [Column("charge_type")]
    [StringLength(30)]
    public string ChargeType { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("room_order_id")]
    public int? RoomOrderId { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("incurred_at")]
    public DateTime? IncurredAt { get; set; }

    [ForeignKey("RoomOrderId")]
    [InverseProperty("ExpenseCharges")]
    public virtual RoomOrder? RoomOrder { get; set; }

    [ForeignKey("StayId")]
    [InverseProperty("ExpenseCharges")]
    public virtual Stay Stay { get; set; } = null!;
}
