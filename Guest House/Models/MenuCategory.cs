using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class MenuCategory
{
    public int MenuCategoryId { get; set; }

    public int HotelId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
