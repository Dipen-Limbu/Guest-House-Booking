using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("room_order_item")]
public partial class RoomOrderItem
{
    [Key]
    [Column("order_item_id")]
    public int OrderItemId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    [Column("total_price", TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    [Column("special_instruction")]
    [StringLength(255)]
    public string? SpecialInstruction { get; set; }

    [ForeignKey("MenuItemId")]
    [InverseProperty("RoomOrderItems")]
    public virtual MenuItem MenuItem { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("RoomOrderItems")]
    public virtual RoomOrder Order { get; set; } = null!;
}
