using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public int MenuCategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual MenuCategory MenuCategory { get; set; } = null!;

    public virtual ICollection<RoomOrderItem> RoomOrderItems { get; set; } = new List<RoomOrderItem>();
}
