using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("booking")]
[Index("BookingReference", Name = "UQ_booking_reference", IsUnique = true)]
public partial class Booking
{
    [Key]
    [Column("booking_id")]
    public int BookingId { get; set; }

    [Column("guest_id")]
    public int GuestId { get; set; }

    [Column("booking_reference")]
    [StringLength(50)]
    public string BookingReference { get; set; } = null!;

    [Column("booking_source")]
    [StringLength(20)]
    public string BookingSource { get; set; } = null!;

    [Column("check_in_date")]
    public DateTime CheckInDate { get; set; }

    [Column("expected_checkout")]
    public DateTime ExpectedCheckout { get; set; }

    [Column("booking_status")]
    [StringLength(20)]
    public string BookingStatus { get; set; } = null!;

    [Column("special_request")]
    public string? SpecialRequest { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    [ForeignKey("GuestId")]
    [InverseProperty("Bookings")]
    public virtual Guest Guest { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual Invoice? Invoice { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Booking")]
    public virtual Stay? Stay { get; set; }
}
