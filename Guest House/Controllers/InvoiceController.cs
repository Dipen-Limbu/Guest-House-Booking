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
    /// Automatic bill generation and invoice lifecycle management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Retrieves all invoices, optionally filtered by status ('unpaid', 'partially_paid', 'paid', 'cancelled')
        /// </summary>
        /// <param name="status">Optional status filter</param>
        /// <response code="200">List of invoices retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InvoiceResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync(status);
            return Ok(ApiResponse<IEnumerable<InvoiceResponseDto>>.SuccessResponse(invoices, "Invoices retrieved successfully."));
        }

        /// <summary>
        /// Retrieves an invoice by its primary ID, including all breakdown items
        /// </summary>
        /// <param name="id">The positive invoice ID</param>
        /// <response code="200">Invoice retrieved successfully</response>
        /// <response code="400">Invalid invoice ID</response>
        /// <response code="404">Invoice not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive invoice ID is required."));
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Invoice with ID {id} was not found."));
            }

            return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(invoice, "Invoice retrieved successfully."));
        }

        /// <summary>
        /// Retrieves the invoice associated with a specific booking
        /// </summary>
        /// <param name="bookingId">The positive booking ID</param>
        /// <response code="200">Invoice found for the booking</response>
        /// <response code="400">Invalid booking ID</response>
        /// <response code="404">No invoice found for the booking</response>
        [HttpGet("booking/{bookingId}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            if (bookingId <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive booking ID is required."));
            }

            var invoice = await _invoiceService.GetInvoiceByBookingIdAsync(bookingId);
            if (invoice == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No invoice found for booking ID {bookingId}."));
            }

            return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(invoice, "Invoice retrieved successfully."));
        }

        /// <summary>
        /// Automatically generates an invoice for a completed or active booking.
        /// Aggregates room charges, customer expense charges, calculates tax/discounts/dues, and generates itemized breakdown.
        /// </summary>
        /// <param name="request">Generation parameters including booking ID and optional tax/discount/advance payments</param>
        /// <response code="201">Invoice generated successfully</response>
        /// <response code="400">Invalid booking state or calculation parameters</response>
        /// <response code="404">Booking not found</response>
        /// <response code="409">Invoice already exists for this booking</response>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GenerateInvoice([FromBody] GenerateInvoiceRequestDto request)
        {
            var result = await _invoiceService.GenerateInvoiceAsync(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.InvoiceId },
                ApiResponse<InvoiceResponseDto>.SuccessResponse(result.Data!, result.Message));
        }

        /// <summary>
        /// Updates invoice financial fields (tax, discount, paid amount, status).
        /// Compatible with the Payment module for recording payments and updating dues.
        /// </summary>
        /// <param name="id">The positive invoice ID</param>
        /// <param name="dto">Update payload</param>
        /// <response code="200">Invoice updated successfully</response>
        /// <response code="400">Validation failure or invalid invoice ID</response>
        /// <response code="404">Invoice not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("A valid positive invoice ID is required."));
            }

            var result = await _invoiceService.UpdateInvoiceAsync(id, dto);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<object>.ErrorResponse(result.Message, result.Errors));
            }

            return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(result.Data!, result.Message));
        }
    }
}
