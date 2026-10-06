using System.Collections.Generic;
using System.Threading.Tasks;
using Guest_House.DTOs.Billing;
using Guest_House.DTOs.Common;
using Guest_House.Services.Billing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    /// <summary>
    /// Management of customer expense charges incurred during a stay (laundry, minibar, room order, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExpenseChargeController : ControllerBase
    {
        private readonly IExpenseChargeService _expenseChargeService;

        public ExpenseChargeController(IExpenseChargeService expenseChargeService)
        {
            _expenseChargeService = expenseChargeService;
        }

        /// <summary>
        /// Retrieves all customer expense charges, optionally filtered by Stay ID
        /// </summary>
        /// <param name="stayId">Optional Stay ID filter</param>
        /// <response code="200">List of expense charges retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpenseChargeResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? stayId = null)
        {
            var charges = await _expenseChargeService.GetAllAsync(stayId);
            return Ok(ApiResponse<IEnumerable<ExpenseChargeResponseDto>>.SuccessResponse(charges, "Expense charges retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific customer expense charge by its primary ID
        /// </summary>
        /// <param name="id">The positive charge ID</param>
        /// <response code="200">Expense charge found and returned</response>
        /// <response code="400">Invalid charge ID</response>
        /// <response code="404">Expense charge not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExpenseChargeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive expense charge ID is required."));
            }

            var charge = await _expenseChargeService.GetByIdAsync(id);
            if (charge == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Expense charge with ID {id} was not found."));
            }

            return Ok(ApiResponse<ExpenseChargeResponseDto>.SuccessResponse(charge, "Expense charge retrieved successfully."));
        }

        /// <summary>
        /// Creates a new customer expense charge for a stay
        /// </summary>
        /// <param name="dto">Expense charge payload</param>
        /// <response code="201">Expense charge created successfully</response>
        /// <response code="400">Invalid payload, negative amount, or invalid charge type / relationships</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExpenseChargeResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ExpenseChargeCreateDto dto)
        {
            var result = await _expenseChargeService.CreateAsync(dto);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.ChargeId },
                ApiResponse<ExpenseChargeResponseDto>.SuccessResponse(result.Data!, result.Message));
        }

        /// <summary>
        /// Updates an existing customer expense charge
        /// </summary>
        /// <param name="id">The positive charge ID</param>
        /// <param name="dto">Updated expense charge values</param>
        /// <response code="200">Expense charge updated successfully</response>
        /// <response code="400">Validation failure or invalid charge ID</response>
        /// <response code="404">Expense charge not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExpenseChargeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseChargeUpdateDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive expense charge ID is required."));
            }

            var result = await _expenseChargeService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return Ok(ApiResponse<ExpenseChargeResponseDto>.SuccessResponse(result.Data!, result.Message));
        }

        /// <summary>
        /// Deletes a customer expense charge
        /// </summary>
        /// <param name="id">The positive charge ID</param>
        /// <response code="200">Expense charge deleted successfully</response>
        /// <response code="400">Invalid charge ID</response>
        /// <response code="404">Expense charge not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive expense charge ID is required."));
            }

            var result = await _expenseChargeService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return Ok(ApiResponse.SuccessResult(result.Message));
        }
    }
}
