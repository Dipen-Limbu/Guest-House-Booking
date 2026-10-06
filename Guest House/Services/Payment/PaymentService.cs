using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Payment;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using PaymentEntity = Guest_House.Models.Payment;

namespace Guest_House.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly GuestHouseContext _context;

        public PaymentService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentResponseDto>> GetAllAsync(int? bookingId, string? paymentStatus, string? paymentMethod, CancellationToken cancellationToken = default)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Include(p => p.Booking)
                .Include(p => p.Invoice)
                .AsQueryable();

            if (bookingId.HasValue)
                query = query.Where(p => p.BookingId == bookingId.Value);

            if (!string.IsNullOrWhiteSpace(paymentStatus))
                query = query.Where(p => p.PaymentStatus.ToLower() == paymentStatus.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(paymentMethod))
                query = query.Where(p => p.PaymentMethod.ToLower() == paymentMethod.Trim().ToLower());

            var payments = await query
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync(cancellationToken);

            var dtos = new List<PaymentResponseDto>();
            foreach (var p in payments)
            {
                dtos.Add(await MapToDtoAsync(p, cancellationToken));
            }

            return dtos;
        }

        public async Task<PaymentResponseDto> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Booking)
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment record with ID {paymentId} was not found.");

            return await MapToDtoAsync(payment, cancellationToken);
        }

        public async Task<BookingPaymentSummaryDto> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Invoice)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking with ID {bookingId} was not found.");

            var payments = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Booking)
                .Include(p => p.Invoice)
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync(cancellationToken);

            var totalBookingAmount = CalculateTotalBookingAmount(booking);
            var completedPayments = payments.Where(p => p.PaymentStatus.ToLower() == "completed").ToList();
            var totalPaidAmount = completedPayments.Sum(p => p.Amount);
            var remainingDue = Math.Max(0, totalBookingAmount - totalPaidAmount);

            var paymentDtos = new List<PaymentResponseDto>();
            foreach (var p in payments)
            {
                paymentDtos.Add(await MapToDtoAsync(p, cancellationToken));
            }

            return new BookingPaymentSummaryDto
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                TotalBookingAmount = totalBookingAmount,
                TotalPaidAmount = totalPaidAmount,
                RemainingDueAmount = remainingDue,
                Payments = paymentDtos
            };
        }

        public async Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
        {
            var booking = await _context.Bookings
                .Include(b => b.Invoice)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(b => b.BookingId == dto.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking with ID {dto.BookingId} was not found.");

            var totalBookingAmount = CalculateTotalBookingAmount(booking);
            var totalPaidSoFar = await _context.Payments
                .Where(p => p.BookingId == dto.BookingId && p.PaymentStatus.ToLower() == "completed")
                .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

            var remainingDue = Math.Max(0, totalBookingAmount - totalPaidSoFar);

            var status = string.IsNullOrWhiteSpace(dto.PaymentStatus) ? "completed" : dto.PaymentStatus.Trim().ToLower();
            var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException($"Invalid payment status '{dto.PaymentStatus}'. Allowed: {string.Join(", ", validStatuses)}.");

            var validTypes = new[] { "advance", "partial", "full" };
            var paymentType = dto.PaymentType.Trim().ToLower();
            if (!validTypes.Contains(paymentType))
                throw new ArgumentException($"Invalid payment type '{dto.PaymentType}'. Allowed: {string.Join(", ", validTypes)}.");

            // Safety check if completing payment
            if (status == "completed")
            {
                if (dto.Amount > remainingDue + 0.01m)
                {
                    throw new InvalidOperationException(
                        $"Payment amount {dto.Amount:N2} exceeds remaining due amount of {remainingDue:N2}. Total booking amount: {totalBookingAmount:N2}, Already paid: {totalPaidSoFar:N2}.");
                }

                if (paymentType == "full" && Math.Abs(dto.Amount - remainingDue) > 0.01m)
                {
                    throw new InvalidOperationException(
                        $"For full payment, amount must equal the remaining balance of {remainingDue:N2}. Specified amount: {dto.Amount:N2}.");
                }
            }

            var payment = new PaymentEntity
            {
                BookingId = dto.BookingId,
                InvoiceId = dto.InvoiceId ?? booking.Invoice?.InvoiceId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod.Trim().ToLower(),
                PaymentType = paymentType,
                PaymentStatus = status,
                TransactionRef = dto.TransactionRef?.Trim(),
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            // Sync invoice amounts if invoice exists and payment is completed
            if (status == "completed" && booking.Invoice != null)
            {
                await SyncInvoicePaymentStatusAsync(booking.Invoice.InvoiceId, cancellationToken);
            }

            return await GetByIdAsync(payment.PaymentId, cancellationToken);
        }

        public async Task<PaymentResponseDto> UpdateAsync(int paymentId, UpdatePaymentDto dto, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Invoice)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                            .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment record with ID {paymentId} was not found.");

            if (dto.Amount.HasValue && dto.Amount.Value <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.");

            if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
                payment.PaymentMethod = dto.PaymentMethod.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(dto.PaymentType))
            {
                var validTypes = new[] { "advance", "partial", "full" };
                var type = dto.PaymentType.Trim().ToLower();
                if (!validTypes.Contains(type))
                    throw new ArgumentException($"Invalid payment type '{dto.PaymentType}'. Allowed: {string.Join(", ", validTypes)}.");
                payment.PaymentType = type;
            }

            if (!string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
                var status = dto.PaymentStatus.Trim().ToLower();
                if (!validStatuses.Contains(status))
                    throw new ArgumentException($"Invalid payment status '{dto.PaymentStatus}'. Allowed: {string.Join(", ", validStatuses)}.");
                payment.PaymentStatus = status;
            }

            if (dto.TransactionRef != null)
                payment.TransactionRef = dto.TransactionRef.Trim();

            if (dto.Amount.HasValue)
            {
                payment.Amount = dto.Amount.Value;
            }

            // Perform safety calculation check if payment status is completed
            if (payment.PaymentStatus == "completed")
            {
                var totalBookingAmount = CalculateTotalBookingAmount(payment.Booking);
                var otherPaymentsSum = await _context.Payments
                    .Where(p => p.BookingId == payment.BookingId && p.PaymentId != paymentId && p.PaymentStatus.ToLower() == "completed")
                    .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

                var remainingDue = Math.Max(0, totalBookingAmount - otherPaymentsSum);

                if (payment.Amount > remainingDue + 0.01m)
                {
                    throw new InvalidOperationException(
                        $"Updated payment amount {payment.Amount:N2} exceeds remaining due amount of {remainingDue:N2}. Total booking amount: {totalBookingAmount:N2}, Other completed payments: {otherPaymentsSum:N2}.");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            if (payment.Booking.Invoice != null)
            {
                await SyncInvoicePaymentStatusAsync(payment.Booking.Invoice.InvoiceId, cancellationToken);
            }

            return await GetByIdAsync(paymentId, cancellationToken);
        }

        public async Task DeleteAsync(int paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment record with ID {paymentId} was not found.");

            var invoiceId = payment.InvoiceId ?? payment.Booking?.Invoice?.InvoiceId;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync(cancellationToken);

            if (invoiceId.HasValue)
            {
                await SyncInvoicePaymentStatusAsync(invoiceId.Value, cancellationToken);
            }
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private decimal CalculateTotalBookingAmount(Booking booking)
        {
            if (booking.Invoice != null && booking.Invoice.GrandTotal > 0)
            {
                return booking.Invoice.GrandTotal;
            }

            // Calculate estimated room charges if invoice doesn't exist
            var days = (int)Math.Max(1, (booking.ExpectedCheckout - booking.CheckInDate).TotalDays);
            var roomTotal = booking.BookingRooms?.Sum(br => br.Room?.Category?.BasePrice ?? 0m) ?? 0m;

            return roomTotal * days;
        }

        private async Task SyncInvoicePaymentStatusAsync(int invoiceId, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
            if (invoice == null) return;

            var totalPaid = await _context.Payments
                .Where(p => p.InvoiceId == invoiceId && p.PaymentStatus.ToLower() == "completed")
                .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

            invoice.PaidAmount = totalPaid;
            invoice.DueAmount = Math.Max(0, invoice.GrandTotal - totalPaid);

            if (invoice.DueAmount <= 0 && invoice.GrandTotal > 0)
                invoice.InvoiceStatus = "paid";
            else if (totalPaid > 0)
                invoice.InvoiceStatus = "partially_paid";
            else
                invoice.InvoiceStatus = "unpaid";

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<PaymentResponseDto> MapToDtoAsync(PaymentEntity p, CancellationToken cancellationToken)
        {
            var booking = p.Booking ?? await _context.Bookings
                .Include(b => b.Invoice)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(b => b.BookingId == p.BookingId, cancellationToken);

            decimal? totalBooking = null;
            decimal? totalPaid = null;
            decimal? remainingDue = null;

            if (booking != null)
            {
                totalBooking = CalculateTotalBookingAmount(booking);
                totalPaid = await _context.Payments
                    .Where(pay => pay.BookingId == p.BookingId && pay.PaymentStatus.ToLower() == "completed")
                    .SumAsync(pay => (decimal?)pay.Amount, cancellationToken) ?? 0m;
                remainingDue = Math.Max(0, totalBooking.Value - totalPaid.Value);
            }

            return new PaymentResponseDto
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                InvoiceId = p.InvoiceId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentType = p.PaymentType,
                PaymentStatus = p.PaymentStatus,
                TransactionRef = p.TransactionRef,
                PaidAt = p.PaidAt,
                BookingReference = booking?.BookingReference,
                TotalBookingAmount = totalBooking,
                TotalPaidAmount = totalPaid,
                RemainingDueAmount = remainingDue
            };
        }
    }
}
