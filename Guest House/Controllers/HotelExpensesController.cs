using System;
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
    [Route("api/hotel-expenses")]
    [Authorize]
    public class HotelExpensesController : ControllerBase
    {
        private readonly IHotelExpenseService _expenseService;

        public HotelExpensesController(IHotelExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Retrieves hotel expenses with optional filtering
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<HotelExpenseResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? hotelId,
            [FromQuery] string? category,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            CancellationToken cancellationToken)
        {
            var data = await _expenseService.GetAllAsync(hotelId, category, startDate, endDate, cancellationToken);
            return Ok(ApiResponse<List<HotelExpenseResponseDto>>.SuccessResponse(data, "Hotel expenses retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific hotel expense by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _expenseService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<HotelExpenseResponseDto>.SuccessResponse(data, "Hotel expense record retrieved successfully."));
        }

        /// <summary>
        /// Retrieves expense summary and calculated totals for a hotel
        /// </summary>
        [HttpGet("summary/{hotelId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummary(
            int hotelId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            CancellationToken cancellationToken)
        {
            var data = await _expenseService.GetSummaryAsync(hotelId, startDate, endDate, cancellationToken);
            return Ok(ApiResponse<HotelExpenseSummaryDto>.SuccessResponse(data, "Hotel expense summary retrieved successfully."));
        }

        /// <summary>
        /// Creates a new hotel expense record
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateHotelExpenseDto dto, CancellationToken cancellationToken)
        {
            var data = await _expenseService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.ExpenseId },
                ApiResponse<HotelExpenseResponseDto>.SuccessResponse(data, "Hotel expense recorded successfully."));
        }

        /// <summary>
        /// Updates an existing hotel expense record
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelExpenseDto dto, CancellationToken cancellationToken)
        {
            var data = await _expenseService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<HotelExpenseResponseDto>.SuccessResponse(data, "Hotel expense updated successfully."));
        }

        /// <summary>
        /// Deletes a hotel expense record
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _expenseService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Hotel expense deleted successfully."));
        }
    }
}
