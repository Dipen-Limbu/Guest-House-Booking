using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.Hotel;
using Guest_House.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly HotelService _hotelService;

        public HotelsController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        private bool IsAuthorizedForHotel(int hotelId)
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
            if (role == "Admin") return true;

            var userHotelIdClaim = User.FindFirst("hotel_id")?.Value;
            if (int.TryParse(userHotelIdClaim, out int userHotelId))
            {
                return userHotelId == hotelId;
            }
            return false;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HotelDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
            var hotels = await _hotelService.GetAllHotelsAsync();

            if (role != "Admin")
            {
                var userHotelIdClaim = User.FindFirst("hotel_id")?.Value;
                if (int.TryParse(userHotelIdClaim, out int userHotelId))
                {
                    hotels = hotels.Where(h => h.HotelId == userHotelId).ToList();
                }
                else
                {
                    return Forbid();
                }
            }

            return Ok(ApiResponse<IEnumerable<HotelDto>>.SuccessResponse(hotels, "Hotels retrieved successfully."));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HotelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (!IsAuthorizedForHotel(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to view this hotel."));
            }

            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Hotel with ID {id} not found."));
            }

            return Ok(ApiResponse<HotelDto>.SuccessResponse(hotel, "Hotel retrieved successfully."));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<HotelDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] HotelCreateDto request)
        {
            var newHotel = await _hotelService.CreateHotelAsync(request);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<HotelDto>.SuccessResponse(newHotel, "Hotel created successfully."));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HotelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateDto request)
        {
            if (!IsAuthorizedForHotel(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to modify this hotel."));
            }

            var updatedHotel = await _hotelService.UpdateHotelAsync(id, request);
            if (updatedHotel == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Hotel with ID {id} not found."));
            }

            return Ok(ApiResponse<HotelDto>.SuccessResponse(updatedHotel, "Hotel updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _hotelService.DeleteHotelAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Hotel with ID {id} not found."));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hotel deleted successfully."));
        }
    }
}
