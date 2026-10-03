using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public int CategoryId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int? FloorNumber { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public virtual RoomCategory Category { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<RoomMedium> RoomMedia { get; set; } = new List<RoomMedium>();

    public virtual ICollection<RoomOrder> RoomOrders { get; set; } = new List<RoomOrder>();
}
