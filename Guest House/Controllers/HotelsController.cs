using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.Hotel;
using Guest_House.Services.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        /// <summary>
        /// Retrieves all hotels
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<HotelResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var data = await _hotelService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<List<HotelResponseDto>>.SuccessResponse(data, "Hotels retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific hotel by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<HotelResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _hotelService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<HotelResponseDto>.SuccessResponse(data, "Hotel retrieved successfully."));
        }

        /// <summary>
        /// Creates a new hotel record
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<HotelResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto, CancellationToken cancellationToken)
        {
            var data = await _hotelService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.HotelId },
                ApiResponse<HotelResponseDto>.SuccessResponse(data, "Hotel created successfully."));
        }

        /// <summary>
        /// Updates an existing hotel's information
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<HotelResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto, CancellationToken cancellationToken)
        {
            var data = await _hotelService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<HotelResponseDto>.SuccessResponse(data, "Hotel updated successfully."));
        }

        /// <summary>
        /// Deletes a hotel record
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _hotelService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Hotel deleted successfully."));
        }
    }
}
