using Guest_House.Models;
using Guest_House.Services;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly GuestService _guestService;

        public GuestController(GuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _guestService.GetAllGuestsAsync();
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            if (guest == null) return NotFound();
            return Ok(guest);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Guest guest)
        {
            var existing = await _guestService.GetGuestByPhoneAsync(guest.Phone);
            if (existing != null)
                return Conflict("A guest with this phone number already exists.");

            var newId = await _guestService.CreateGuestAsync(guest);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { guestId = newId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Guest guest)
        {
            guest.GuestId = id;
            var success = await _guestService.UpdateGuestAsync(guest);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
