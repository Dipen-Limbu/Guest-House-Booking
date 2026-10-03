using Guest_House.DTOs.Common;
using Guest_House.DTOs.Room;
using Guest_House.Services.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [Route("api/room-categories")]
    [ApiController]
    [Authorize]
    public class RoomCategoriesController : ControllerBase
    {
        private readonly RoomCategoryService _service;
        public RoomCategoriesController(RoomCategoryService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? hotelId)
        {
            var data = await _service.GetAllAsync(hotelId);
            return Ok(ApiResponse<List<RoomCategoryResponseDto>>.SuccessResponse(data, "Room categories retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<RoomCategoryResponseDto>.SuccessResponse(data, "Room category retrieved successfully."));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateRoomCategoryDto dto)
        {
            var data = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.CategoryId },
                ApiResponse<RoomCategoryResponseDto>.SuccessResponse(data, "Room category created successfully."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomCategoryDto dto)
        {
            var data = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoomCategoryResponseDto>.SuccessResponse(data, "Room category updated successfully."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResult("Room category deleted successfully."));
        }
    }
}
