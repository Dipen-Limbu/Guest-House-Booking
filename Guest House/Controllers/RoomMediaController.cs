using Guest_House.DTOs.Common;
using Guest_House.DTOs.Room;
using Guest_House.Services.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Authorize]
    public class RoomMediaController : ControllerBase       // no class-level [Route]; each action has a full route
    {
        private readonly RoomMediaService _service;
        public RoomMediaController(RoomMediaService service) => _service = service;

        [HttpGet("api/rooms/{roomId:int}/media")]
        public async Task<IActionResult> GetByRoom(int roomId)
        {
            var data = await _service.GetByRoomAsync(roomId);
            return Ok(ApiResponse<List<RoomMediaResponseDto>>.SuccessResponse(data, "Room media retrieved successfully."));
        }

        [HttpPost("api/rooms/{roomId:int}/media")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(int roomId, [FromBody] CreateRoomMediaDto dto)
        {
            var data = await _service.CreateAsync(roomId, dto);
            return CreatedAtAction(nameof(GetById), new { mediaId = data.MediaId },
                ApiResponse<RoomMediaResponseDto>.SuccessResponse(data, "Room media added successfully."));
        }

        [HttpGet("api/room-media/{mediaId:int}")]
        public async Task<IActionResult> GetById(int mediaId)
        {
            var data = await _service.GetByIdAsync(mediaId);
            return Ok(ApiResponse<RoomMediaResponseDto>.SuccessResponse(data, "Room media retrieved successfully."));
        }

        [HttpPut("api/room-media/{mediaId:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int mediaId, [FromBody] UpdateRoomMediaDto dto)
        {
            var data = await _service.UpdateAsync(mediaId, dto);
            return Ok(ApiResponse<RoomMediaResponseDto>.SuccessResponse(data, "Room media updated successfully."));
        }

        [HttpDelete("api/room-media/{mediaId:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int mediaId)
        {
            await _service.DeleteAsync(mediaId);
            return Ok(ApiResponse.SuccessResult("Room media deleted successfully."));
        }
    }
}
