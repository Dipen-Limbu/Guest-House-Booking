using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Booking;

public class CreateBookingDto
{
    [Required]
    public int GuestId { get; set; }

    [Required]
    public string BookingReference { get; set; } = null!;

    [Required]
    public string BookingSource { get; set; } = null!;

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime ExpectedCheckout { get; set; }

    public string? BookingStatus { get; set; }

    public string? SpecialRequest { get; set; }

    [Required]
    public List<CreateBookingRoomDto> Rooms { get; set; } = new();
}

public class CreateBookingRoomDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public decimal RoomPrice { get; set; }

    public int? NumberOfGuests { get; set; }
}