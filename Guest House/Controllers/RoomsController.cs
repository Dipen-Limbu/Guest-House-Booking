using Guest_House.DTOs.Common;
using Guest_House.DTOs.Room;
using Guest_House.Services.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly RoomService _service;
        public RoomsController(RoomService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? hotelId, [FromQuery] int? categoryId, [FromQuery] string? status)
        {
            var data = await _service.GetAllAsync(hotelId, categoryId, status);
            return Ok(ApiResponse<List<RoomResponseDto>>.SuccessResponse(data, "Rooms retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<RoomResponseDto>.SuccessResponse(data, "Room retrieved successfully."));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var data = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.RoomId },
                ApiResponse<RoomResponseDto>.SuccessResponse(data, "Room created successfully."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
        {
            var data = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoomResponseDto>.SuccessResponse(data, "Room updated successfully."));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRoomStatusDto dto)
        {
            var data = await _service.UpdateStatusAsync(id, dto);
            return Ok(ApiResponse<RoomResponseDto>.SuccessResponse(data, "Room status updated successfully."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResult("Room deleted successfully."));
        }
    }
}
