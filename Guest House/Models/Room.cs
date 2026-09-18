using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("room")]
[Index("HotelId", "RoomNumber", Name = "UQ_room_hotel_number", IsUnique = true)]
public partial class Room
{
    [Key]
    [Column("room_id")]
    public int RoomId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("room_number")]
    [StringLength(20)]
    public string RoomNumber { get; set; } = null!;

    [Column("floor_number")]
    public int? FloorNumber { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Rooms")]
    public virtual RoomCategory Category { get; set; } = null!;

    [ForeignKey("HotelId")]
    [InverseProperty("Rooms")]
    public virtual Hotel Hotel { get; set; } = null!;

    [InverseProperty("Room")]
    public virtual ICollection<RoomMedium> RoomMedia { get; set; } = new List<RoomMedium>();

    [InverseProperty("Room")]
    public virtual ICollection<RoomOrder> RoomOrders { get; set; } = new List<RoomOrder>();
}
