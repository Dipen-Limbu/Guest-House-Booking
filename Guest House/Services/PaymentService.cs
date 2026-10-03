
using Guest_House.Data;
using Guest_House.DTOs.Payment;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Services;

public class PaymentService : IPaymentService
{
    private readonly GuestHouseContext _context;

    public PaymentService(GuestHouseContext context)
    {
        _context = context;
    }


    // =========================================================
    // CREATE PAYMENT
    // =========================================================
    public async Task<PaymentResponseDto> CreatePaymentAsync(
        CreatePaymentDto dto)
    {
        // Check booking
        var bookingExists = await _context.Bookings
            .AnyAsync(b => b.BookingId == dto.BookingId);

        if (!bookingExists)
        {
            throw new Exception("Booking not found.");
        }


        // Check invoice if provided
        if (dto.InvoiceId.HasValue)
        {
            var invoiceExists = await _context.Invoices
                .AnyAsync(i => i.InvoiceId == dto.InvoiceId.Value);

            if (!invoiceExists)
            {
                throw new Exception("Invoice not found.");
            }
        }


        // Create payment
        var payment = new Payment
        {
            BookingId = dto.BookingId,
            InvoiceId = dto.InvoiceId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            PaymentType = dto.PaymentType,

            // New payment starts as pending
            PaymentStatus = "pending",

            TransactionRef = dto.TransactionRef,
            PaidAt = DateTime.Now
        };


        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        return MapToResponse(payment);
    }


    // =========================================================
    // GET PAYMENT BY ID
    // =========================================================
    public async Task<PaymentResponseDto?> GetPaymentByIdAsync(
        int paymentId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

        if (payment == null)
        {
            return null;
        }

        return MapToResponse(payment);
    }


    // =========================================================
    // GET ALL PAYMENTS BY BOOKING
    // =========================================================
    public async Task<List<PaymentResponseDto>> GetPaymentsByBookingAsync(
        int bookingId)
    {
        var payments = await _context.Payments
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.PaymentId)
            .ToListAsync();

        return payments
            .Select(MapToResponse)
            .ToList();
    }


    // =========================================================
    // UPDATE PAYMENT STATUS
    // =========================================================
    public async Task<bool> UpdatePaymentStatusAsync(
        int paymentId,
        string status)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

        if (payment == null)
        {
            return false;
        }


        // Validate status
        var validStatuses = new[]
        {
            "pending",
            "completed",
            "failed",
            "refunded"
        };


        var normalizedStatus = status?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(normalizedStatus) ||
            !validStatuses.Contains(normalizedStatus))
        {
            throw new Exception(
                "Invalid payment status. Allowed values: pending, completed, failed, refunded.");
        }


        // Keep old status
        var oldStatus = payment.PaymentStatus
            .Trim()
            .ToLowerInvariant();


        // If status is already the same,
        // do nothing to invoice amount.
        if (oldStatus == normalizedStatus)
        {
            return true;
        }


        // =====================================================
        // PENDING / FAILED → COMPLETED
        // Add payment amount to invoice
        // =====================================================
        if (normalizedStatus == "completed" &&
            (oldStatus == "pending" || oldStatus == "failed"))
        {
            await AddPaymentToInvoiceAsync(payment);
        }


        // =====================================================
        // COMPLETED → REFUNDED
        // Remove payment amount from invoice
        // =====================================================
        else if (normalizedStatus == "refunded" &&
                 oldStatus == "completed")
        {
            await RemovePaymentFromInvoiceAsync(payment);
        }


        // =====================================================
        // COMPLETED → PENDING
        // Remove previously counted payment
        // =====================================================
        else if (normalizedStatus == "pending" &&
                 oldStatus == "completed")
        {
            await RemovePaymentFromInvoiceAsync(payment);
        }


        // Update payment status
        payment.PaymentStatus = normalizedStatus;


        await _context.SaveChangesAsync();

        return true;
    }


    // =========================================================
    // ADD PAYMENT AMOUNT TO INVOICE
    // =========================================================
    private async Task AddPaymentToInvoiceAsync(
        Payment payment)
    {
        // Payment has no invoice
        if (!payment.InvoiceId.HasValue)
        {
            return;
        }


        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i =>
                i.InvoiceId == payment.InvoiceId.Value);

        if (invoice == null)
        {
            throw new Exception("Invoice not found.");
        }


        // Add payment amount
        invoice.PaidAmount += payment.Amount;


        // Prevent paid amount from going above grand total
        if (invoice.PaidAmount > invoice.GrandTotal)
        {
            invoice.PaidAmount = invoice.GrandTotal;
        }


        // Calculate due amount
        invoice.DueAmount =
            invoice.GrandTotal - invoice.PaidAmount;


        if (invoice.DueAmount < 0)
        {
            invoice.DueAmount = 0;
        }


        // Update invoice status
        if (invoice.DueAmount == 0)
        {
            invoice.InvoiceStatus = "paid";
        }
        else if (invoice.PaidAmount > 0)
        {
            invoice.InvoiceStatus = "partially_paid";
        }
        else
        {
            invoice.InvoiceStatus = "unpaid";
        }
    }


    // =========================================================
    // REMOVE PAYMENT AMOUNT FROM INVOICE
    // =========================================================
    private async Task RemovePaymentFromInvoiceAsync(
        Payment payment)
    {
        // Payment has no invoice
        if (!payment.InvoiceId.HasValue)
        {
            return;
        }


        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i =>
                i.InvoiceId == payment.InvoiceId.Value);

        if (invoice == null)
        {
            throw new Exception("Invoice not found.");
        }


        // Remove payment amount
        invoice.PaidAmount -= payment.Amount;


        // Prevent negative paid amount
        if (invoice.PaidAmount < 0)
        {
            invoice.PaidAmount = 0;
        }


        // Recalculate due amount
        invoice.DueAmount =
            invoice.GrandTotal - invoice.PaidAmount;


        if (invoice.DueAmount < 0)
        {
            invoice.DueAmount = 0;
        }


        // Update invoice status
        if (invoice.DueAmount == 0)
        {
            invoice.InvoiceStatus = "paid";
        }
        else if (invoice.PaidAmount > 0)
        {
            invoice.InvoiceStatus = "partially_paid";
        }
        else
        {
            invoice.InvoiceStatus = "unpaid";
        }
    }


    // =========================================================
    // MAP ENTITY TO RESPONSE DTO
    // =========================================================
    private static PaymentResponseDto MapToResponse(
        Payment payment)
    {
        return new PaymentResponseDto
        {
            PaymentId = payment.PaymentId,
            BookingId = payment.BookingId,
            InvoiceId = payment.InvoiceId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentType = payment.PaymentType,
            PaymentStatus = payment.PaymentStatus,
            TransactionRef = payment.TransactionRef,
            PaidAt = payment.PaidAt
        };
    }
}
