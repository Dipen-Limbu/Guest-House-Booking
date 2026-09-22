namespace Guest_House.Models;

public partial class Booking
{
    public int BookingId { get; set; }
    public int GuestId { get; set; }
    public string BookingReference { get; set; } = null!;
    public string BookingSource { get; set; } = null!;
    public DateTime CheckInDate { get; set; }
    public DateTime ExpectedCheckout { get; set; }
    public string? BookingStatus { get; set; }
    public string? SpecialRequest { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    public virtual Guest Guest { get; set; } = null!;
    public virtual Invoice? Invoice { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual Stay? Stay { get; set; }
}