using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.WebsiteSyncLog;
using Guest_House.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/website-sync-logs")]
    [Authorize]
    public class WebsiteSyncLogsController : ControllerBase
    {
        private readonly WebsiteSyncLogService _logService;

        public WebsiteSyncLogsController(WebsiteSyncLogService logService)
        {
            _logService = logService;
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
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WebsiteSyncLogDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
            
            if (role == "Admin")
            {
                var logs = await _logService.GetAllLogsAsync();
                return Ok(ApiResponse<IEnumerable<WebsiteSyncLogDto>>.SuccessResponse(logs, "Website sync logs retrieved successfully."));
            }
            else
            {
                var userHotelIdClaim = User.FindFirst("hotel_id")?.Value;
                if (int.TryParse(userHotelIdClaim, out int userHotelId))
                {
                    var logs = await _logService.GetLogsByHotelIdAsync(userHotelId);
                    return Ok(ApiResponse<IEnumerable<WebsiteSyncLogDto>>.SuccessResponse(logs, "Website sync logs retrieved successfully."));
                }
                return Forbid();
            }
        }

        [HttpGet("~/api/hotels/{hotelId}/website-sync-logs")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WebsiteSyncLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByHotelId(int hotelId, [FromQuery] string? entityType, [FromQuery] string? syncStatus)
        {
            if (!IsAuthorizedForHotel(hotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to view sync logs for this hotel."));
            }

            var logs = await _logService.GetLogsByHotelIdAsync(hotelId, entityType, syncStatus);
            return Ok(ApiResponse<IEnumerable<WebsiteSyncLogDto>>.SuccessResponse(logs, "Website sync logs retrieved successfully."));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<WebsiteSyncLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _logService.GetLogByIdAsync(id);
            if (log == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Website sync log with ID {id} not found."));
            }

            if (!IsAuthorizedForHotel(log.HotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to view this sync log."));
            }

            return Ok(ApiResponse<WebsiteSyncLogDto>.SuccessResponse(log, "Website sync log retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<WebsiteSyncLogDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] WebsiteSyncLogCreateDto request)
        {
            if (!IsAuthorizedForHotel(request.HotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to create a sync log for this hotel."));
            }

            var newLog = await _logService.CreateLogAsync(request);
            if (newLog == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Hotel with ID {request.HotelId} not found."));
            }

            return StatusCode(StatusCodes.Status201Created, ApiResponse<WebsiteSyncLogDto>.SuccessResponse(newLog, "Website sync log created successfully."));
        }
    }
}
