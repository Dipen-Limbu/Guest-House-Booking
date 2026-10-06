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
    /// Line items associated with invoices (room charges, laundry, minibar, service fees, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceItemController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Retrieves all invoice items, optionally filtered by Invoice ID
        /// </summary>
        /// <param name="invoiceId">Optional Invoice ID filter</param>
        /// <response code="200">List of invoice items retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceItemResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? invoiceId = null)
        {
            var items = await _invoiceService.GetInvoiceItemsAsync(invoiceId);
            return Ok(ApiResponse<IEnumerable<InvoiceItemResponseDto>>.SuccessResponse(items, "Invoice items retrieved successfully."));
        }

        /// <summary>
        /// Retrieves invoice items for a specific invoice
        /// </summary>
        /// <param name="invoiceId">The positive invoice ID</param>
        /// <response code="200">Invoice items retrieved successfully</response>
        /// <response code="400">Invalid invoice ID</response>
        [HttpGet("invoice/{invoiceId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceItemResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByInvoiceId(int invoiceId)
        {
            if (invoiceId <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive invoice ID is required."));
            }

            var items = await _invoiceService.GetInvoiceItemsAsync(invoiceId);
            return Ok(ApiResponse<IEnumerable<InvoiceItemResponseDto>>.SuccessResponse(items, $"Items for invoice {invoiceId} retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific invoice item by its ID
        /// </summary>
        /// <param name="id">The positive invoice item ID</param>
        /// <response code="200">Invoice item found and returned</response>
        /// <response code="400">Invalid invoice item ID</response>
        /// <response code="404">Invoice item not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive invoice item ID is required."));
            }

            var item = await _invoiceService.GetInvoiceItemByIdAsync(id);
            if (item == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Invoice item with ID {id} was not found."));
            }

            return Ok(ApiResponse<InvoiceItemResponseDto>.SuccessResponse(item, "Invoice item retrieved successfully."));
        }

        /// <summary>
        /// Manually appends an invoice item to an open/unpaid invoice and recalculates invoice totals
        /// </summary>
        /// <param name="dto">Invoice item payload</param>
        /// <response code="201">Invoice item created and invoice totals updated successfully</response>
        /// <response code="400">Invalid payload, closed invoice, or invalid item type</response>
        /// <response code="404">Invoice not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InvoiceItemResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] InvoiceItemCreateDto dto)
        {
            var result = await _invoiceService.AddInvoiceItemAsync(dto);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.InvoiceItemId },
                ApiResponse<InvoiceItemResponseDto>.SuccessResponse(result.Data!, result.Message));
        }
    }
}
