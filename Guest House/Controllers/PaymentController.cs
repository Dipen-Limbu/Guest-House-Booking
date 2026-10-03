
using Guest_House.DTOs.Payment;
using Guest_House.Services;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }


    // CREATE PAYMENT
    [HttpPost]
    public async Task<ActionResult<PaymentResponseDto>> CreatePayment(
        CreatePaymentDto dto)
    {
        try
        {
            var payment =
                await _paymentService.CreatePaymentAsync(dto);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { paymentId = payment.PaymentId },
                payment);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // GET PAYMENT BY ID
    [HttpGet("{paymentId:int}")]
    public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(
        int paymentId)
    {
        var payment =
            await _paymentService.GetPaymentByIdAsync(paymentId);

        if (payment == null)
        {
            return NotFound(new
            {
                message = "Payment not found."
            });
        }

        return Ok(payment);
    }


    // GET PAYMENTS BY BOOKING
    [HttpGet("booking/{bookingId:int}")]
    public async Task<ActionResult<List<PaymentResponseDto>>>
        GetPaymentsByBooking(int bookingId)
    {
        var payments =
            await _paymentService.GetPaymentsByBookingAsync(bookingId);

        return Ok(payments);
    }


    // UPDATE PAYMENT STATUS
    [HttpPut("{paymentId:int}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        int paymentId,
        [FromBody] string status)
    {
        try
        {
            var success =
                await _paymentService.UpdatePaymentStatusAsync(
                    paymentId,
                    status);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(new
            {
                message = "Payment status updated successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
