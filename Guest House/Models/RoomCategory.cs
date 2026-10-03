using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class RoomCategory
{
    public int CategoryId { get; set; }

    public int HotelId { get; set; }

    public string CategoryName { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public int MaxOccupancy { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
