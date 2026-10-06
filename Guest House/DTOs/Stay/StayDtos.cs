using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Stay
{
    public class CheckInDto
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }

    public class UpdateStayDto
    {
        [StringLength(50, ErrorMessage = "Stay status cannot exceed 50 characters.")]
        public string? StayStatus { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }

    public class StayResponseDto
    {
        public int StayId { get; set; }
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = null!;
        public int GuestId { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestPhone { get; set; } = null!;
        public DateTime? ActualCheckin { get; set; }
        public DateTime? ActualCheckout { get; set; }
        public string StayStatus { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<StayRoomDetailsDto> Rooms { get; set; } = new();
    }

    public class StayRoomDetailsDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string RoomStatus { get; set; } = null!;
    }
}
