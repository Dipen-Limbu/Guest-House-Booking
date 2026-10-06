using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.Stay;
using Guest_House.Services.Stay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/stays")]
    [Authorize]
    public class StaysController : ControllerBase
    {
        private readonly IStayService _stayService;

        public StaysController(IStayService stayService)
        {
            _stayService = stayService;
        }

        /// <summary>
        /// Performs guest check-in for a booking
        /// </summary>
        [HttpPost("check-in")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<StayResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto, CancellationToken cancellationToken)
        {
            var data = await _stayService.CheckInAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.StayId },
                ApiResponse<StayResponseDto>.SuccessResponse(data, "Guest checked in successfully."));
        }

        /// <summary>
        /// Retrieves stay records with optional filters
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<StayResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? bookingId, [FromQuery] string? stayStatus, CancellationToken cancellationToken)
        {
            var data = await _stayService.GetAllAsync(bookingId, stayStatus, cancellationToken);
            return Ok(ApiResponse<List<StayResponseDto>>.SuccessResponse(data, "Stay records retrieved successfully."));
        }

        /// <summary>
        /// Retrieves all currently active stays
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<StayResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            var data = await _stayService.GetActiveStaysAsync(cancellationToken);
            return Ok(ApiResponse<List<StayResponseDto>>.SuccessResponse(data, "Active stay records retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific stay record by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<StayResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _stayService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<StayResponseDto>.SuccessResponse(data, "Stay record retrieved successfully."));
        }

        /// <summary>
        /// Performs guest check-out for a stay
        /// </summary>
        [HttpPost("{id:int}/check-out")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<StayResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckOut(int id, CancellationToken cancellationToken)
        {
            var data = await _stayService.CheckOutAsync(id, cancellationToken);
            return Ok(ApiResponse<StayResponseDto>.SuccessResponse(data, "Guest checked out successfully."));
        }

        /// <summary>
        /// Updates stay information (e.g. notes or status)
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<StayResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStayDto dto, CancellationToken cancellationToken)
        {
            var data = await _stayService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<StayResponseDto>.SuccessResponse(data, "Stay record updated successfully."));
        }
    }
}
