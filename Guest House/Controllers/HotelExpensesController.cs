using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.HotelExpense;
using Guest_House.Services;
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
        private readonly HotelExpenseService _expenseService;

        public HotelExpensesController(HotelExpenseService expenseService)
        {
            _expenseService = expenseService;
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

        private async Task<bool> IsAuthorizedForExpenseAsync(int expenseId)
        {
            var hotelId = await _expenseService.GetHotelIdForExpenseAsync(expenseId);
            if (hotelId == null) return false;
            return IsAuthorizedForHotel(hotelId.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HotelExpenseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
            
            if (role == "Admin")
            {
                var expenses = await _expenseService.GetAllExpensesAsync();
                return Ok(ApiResponse<IEnumerable<HotelExpenseDto>>.SuccessResponse(expenses, "Hotel expenses retrieved successfully."));
            }
            else
            {
                var userHotelIdClaim = User.FindFirst("hotel_id")?.Value;
                if (int.TryParse(userHotelIdClaim, out int userHotelId))
                {
                    var expenses = await _expenseService.GetExpensesByHotelIdAsync(userHotelId);
                    return Ok(ApiResponse<IEnumerable<HotelExpenseDto>>.SuccessResponse(expenses, "Hotel expenses retrieved successfully."));
                }
                return Forbid();
            }
        }

        [HttpGet("~/api/hotels/{hotelId}/expenses")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HotelExpenseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByHotelId(int hotelId)
        {
            if (!IsAuthorizedForHotel(hotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to view expenses for this hotel."));
            }

            var expenses = await _expenseService.GetExpensesByHotelIdAsync(hotelId);
            return Ok(ApiResponse<IEnumerable<HotelExpenseDto>>.SuccessResponse(expenses, "Hotel expenses retrieved successfully."));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Expense with ID {id} not found."));
            }

            if (!IsAuthorizedForHotel(expense.HotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to view this expense."));
            }

            return Ok(ApiResponse<HotelExpenseDto>.SuccessResponse(expense, "Expense retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] HotelExpenseCreateDto request)
        {
            if (!IsAuthorizedForHotel(request.HotelId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to create an expense for this hotel."));
            }

            var newExpense = await _expenseService.CreateExpenseAsync(request);
            if (newExpense == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Hotel with ID {request.HotelId} not found."));
            }

            return StatusCode(StatusCodes.Status201Created, ApiResponse<HotelExpenseDto>.SuccessResponse(newExpense, "Expense created successfully."));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<HotelExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] HotelExpenseUpdateDto request)
        {
            if (!await IsAuthorizedForExpenseAsync(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to modify this expense."));
            }

            var updatedExpense = await _expenseService.UpdateExpenseAsync(id, request);
            if (updatedExpense == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Expense with ID {id} not found."));
            }

            return Ok(ApiResponse<HotelExpenseDto>.SuccessResponse(updatedExpense, "Expense updated successfully."));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsAuthorizedForExpenseAsync(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("You do not have permission to delete this expense."));
            }

            var success = await _expenseService.DeleteExpenseAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Expense with ID {id} not found."));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Expense deleted successfully."));
        }
    }
}
