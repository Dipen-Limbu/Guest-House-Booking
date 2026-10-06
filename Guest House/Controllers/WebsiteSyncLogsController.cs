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
    [Route("api/website-sync-logs")]
    [Authorize]
    public class WebsiteSyncLogsController : ControllerBase
    {
        private readonly IWebsiteSyncLogService _syncLogService;

        public WebsiteSyncLogsController(IWebsiteSyncLogService syncLogService)
        {
            _syncLogService = syncLogService;
        }

        /// <summary>
        /// Retrieves website sync logs with optional filters
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<WebsiteSyncLogResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? hotelId,
            [FromQuery] string? syncStatus,
            [FromQuery] string? entityType,
            CancellationToken cancellationToken)
        {
            var data = await _syncLogService.GetAllAsync(hotelId, syncStatus, entityType, cancellationToken);
            return Ok(ApiResponse<List<WebsiteSyncLogResponseDto>>.SuccessResponse(data, "Website sync logs retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific website sync log by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<WebsiteSyncLogResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _syncLogService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<WebsiteSyncLogResponseDto>.SuccessResponse(data, "Website sync log record retrieved successfully."));
        }

        /// <summary>
        /// Records a new website synchronization log
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<WebsiteSyncLogResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateWebsiteSyncLogDto dto, CancellationToken cancellationToken)
        {
            var data = await _syncLogService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.SyncId },
                ApiResponse<WebsiteSyncLogResponseDto>.SuccessResponse(data, "Website sync log recorded successfully."));
        }
    }
}
