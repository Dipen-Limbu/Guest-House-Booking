using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.Payment;
using Guest_House.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly EsewaPaymentGatewayService _esewaGateway;
        private readonly KhaltiPaymentGatewayService _khaltiGateway;

        public PaymentsController(
            IPaymentService paymentService,
            EsewaPaymentGatewayService esewaGateway,
            KhaltiPaymentGatewayService khaltiGateway)
        {
            _paymentService = paymentService;
            _esewaGateway = esewaGateway;
            _khaltiGateway = khaltiGateway;
        }

        /// <summary>
        /// Retrieves payment records with optional filters
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? bookingId, [FromQuery] string? paymentStatus, [FromQuery] string? paymentMethod, CancellationToken cancellationToken)
        {
            var data = await _paymentService.GetAllAsync(bookingId, paymentStatus, paymentMethod, cancellationToken);
            return Ok(ApiResponse<List<PaymentResponseDto>>.SuccessResponse(data, "Payments retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific payment record by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _paymentService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(data, "Payment record retrieved successfully."));
        }

        /// <summary>
        /// Retrieves all payments and payment summary for a specific booking
        /// </summary>
        [HttpGet("booking/{bookingId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<BookingPaymentSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByBookingId(int bookingId, CancellationToken cancellationToken)
        {
            var data = await _paymentService.GetByBookingIdAsync(bookingId, cancellationToken);
            return Ok(ApiResponse<BookingPaymentSummaryDto>.SuccessResponse(data, "Booking payment details retrieved successfully."));
        }

        /// <summary>
        /// Creates a new payment record (Advance, Partial, or Full payment)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            var data = await _paymentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.PaymentId },
                ApiResponse<PaymentResponseDto>.SuccessResponse(data, "Payment recorded successfully."));
        }

        /// <summary>
        /// Updates an existing payment record
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentDto dto, CancellationToken cancellationToken)
        {
            var data = await _paymentService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(data, "Payment updated successfully."));
        }

        /// <summary>
        /// Deletes a payment record
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _paymentService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Payment deleted successfully."));
        }

        // ==========================================
        // eSEWA & KHALTI PAYMENT GATEWAY ENDPOINTS
        // ==========================================

        /// <summary>
        /// Initiates eSewa payment transaction flow
        /// </summary>
        [HttpPost("esewa/initiate")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentGatewayInitiateResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> InitiateEsewa([FromBody] EsewaInitiateRequestDto dto, CancellationToken cancellationToken)
        {
            var data = await _esewaGateway.InitiatePaymentAsync(dto.BookingId, dto.Amount, dto.SuccessUrl, cancellationToken);
            return Ok(ApiResponse<PaymentGatewayInitiateResponseDto>.SuccessResponse(data, data.Message));
        }

        /// <summary>
        /// Verifies eSewa payment response and records payment in system
        /// </summary>
        [HttpPost("esewa/verify")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentGatewayVerifyResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEsewa([FromBody] EsewaVerifyRequestDto dto, CancellationToken cancellationToken)
        {
            var data = await _esewaGateway.VerifyPaymentAsync(dto.BookingId, dto.TransactionRef, dto.Amount, dto.EncodedResponse, cancellationToken);
            return Ok(ApiResponse<PaymentGatewayVerifyResponseDto>.SuccessResponse(data, data.Message));
        }

        /// <summary>
        /// Initiates Khalti payment transaction flow
        /// </summary>
        [HttpPost("khalti/initiate")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentGatewayInitiateResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> InitiateKhalti([FromBody] KhaltiInitiateRequestDto dto, CancellationToken cancellationToken)
        {
            var data = await _khaltiGateway.InitiatePaymentAsync(dto.BookingId, dto.Amount, dto.ReturnUrl, cancellationToken);
            return Ok(ApiResponse<PaymentGatewayInitiateResponseDto>.SuccessResponse(data, data.Message));
        }

        /// <summary>
        /// Verifies Khalti payment status and records payment in system
        /// </summary>
        [HttpPost("khalti/verify")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<PaymentGatewayVerifyResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyKhalti([FromBody] KhaltiVerifyRequestDto dto, CancellationToken cancellationToken)
        {
            var data = await _khaltiGateway.VerifyPaymentAsync(dto.BookingId, dto.pidx, dto.Amount, dto.TransactionId, cancellationToken);
            return Ok(ApiResponse<PaymentGatewayVerifyResponseDto>.SuccessResponse(data, data.Message));
        }
    }
}
