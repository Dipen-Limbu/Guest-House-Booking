using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("booking_room")]
[Index("BookingId", "RoomId", Name = "UQ_booking_room", IsUnique = true)]
public partial class BookingRoom
{
    [Key]
    [Column("booking_room_id")]
    public int BookingRoomId { get; set; }

    [Column("booking_id")]
    public int BookingId { get; set; }

    [Column("room_id")]
    public int RoomId { get; set; }

    [Column("room_price", TypeName = "decimal(10, 2)")]
    public decimal RoomPrice { get; set; }

    [Column("number_of_guests")]
    public int NumberOfGuests { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingRooms")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("BookingRooms")]
    public virtual Room Room { get; set; } = null!;
}
