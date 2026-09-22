using Guest_House.Models;
using Guest_House.Services;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET api/booking/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var validStatuses = new[] { "pending", "confirmed", "cancelled", "checked_in", "checked_out", "no_show" };
            if (!validStatuses.Contains(newStatus))
                return BadRequest("Invalid booking status.");

            var success = await _bookingService.UpdateBookingStatusAsync(id, newStatus);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET api/booking/available-rooms?hotelId=1&checkIn=2026-10-01&checkOut=2026-10-05
        [HttpGet("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            var roomIds = await _bookingService.GetAvailableRoomIdsAsync(hotelId, checkIn, checkOut);
            return Ok(roomIds);
        }

        // POST api/booking
        public class CreateBookingRequest
        {
            public Booking Booking { get; set; } = null!;
            public List<BookingRoom> Rooms { get; set; } = null!;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            if (request.Rooms == null || request.Rooms.Count == 0)
                return BadRequest("At least one room must be included in the booking.");

            var newBookingId = await _bookingService.CreateBookingAsync(request.Booking, request.Rooms);
            return CreatedAtAction(nameof(GetById), new { id = newBookingId }, new { bookingId = newBookingId });
        }

        
        
    }
}