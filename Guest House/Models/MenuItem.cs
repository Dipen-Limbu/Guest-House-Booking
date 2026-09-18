using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("menu_item")]
public partial class MenuItem
{
    [Key]
    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("menu_category_id")]
    public int MenuCategoryId { get; set; }

    [Column("item_name")]
    [StringLength(150)]
    public string ItemName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price", TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column("image_url")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MenuCategoryId")]
    [InverseProperty("MenuItems")]
    public virtual MenuCategory MenuCategory { get; set; } = null!;

    [InverseProperty("MenuItem")]
    public virtual ICollection<RoomOrderItem> RoomOrderItems { get; set; } = new List<RoomOrderItem>();
}
