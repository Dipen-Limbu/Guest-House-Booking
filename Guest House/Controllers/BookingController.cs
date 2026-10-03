using Guest_House.DTOs.Booking;
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

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // PUT api/booking/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] string newStatus)
        {
            var validStatuses = new[]
            {
                "pending",
                "confirmed",
                "cancelled",
                "checked_in",
                "checked_out",
                "no_show"
            };

            if (!validStatuses.Contains(newStatus))
                return BadRequest("Invalid booking status.");

            var success =
                await _bookingService.UpdateBookingStatusAsync(id, newStatus);

            if (!success)
                return NotFound();

            return NoContent();
        }

        // GET api/booking/available-rooms
        [HttpGet("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms(
            int hotelId,
            DateTime checkIn,
            DateTime checkOut)
        {
            var roomIds =
                await _bookingService.GetAvailableRoomIdsAsync(
                    hotelId,
                    checkIn,
                    checkOut);

            return Ok(roomIds);
        }

        // POST api/booking
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBookingDto dto)
        {
            if (dto.Rooms == null || dto.Rooms.Count == 0)
            {
                return BadRequest(
                    "At least one room must be included in the booking.");
            }

            var booking = new Models.Booking
            {
                GuestId = dto.GuestId,
                BookingReference = dto.BookingReference,
                BookingSource = dto.BookingSource,
                CheckInDate = dto.CheckInDate,
                ExpectedCheckout = dto.ExpectedCheckout,
                BookingStatus = dto.BookingStatus,
                SpecialRequest = dto.SpecialRequest,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var rooms = dto.Rooms.Select(r => new Models.BookingRoom
            {
                RoomId = r.RoomId,
                RoomPrice = r.RoomPrice,
                NumberOfGuests = r.NumberOfGuests
            }).ToList();

            var newBookingId =
                await _bookingService.CreateBookingAsync(booking, rooms);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newBookingId },
                new
                {
                    bookingId = newBookingId
                });
        }
    }
}