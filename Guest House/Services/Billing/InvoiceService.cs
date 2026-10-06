using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Billing;
using Guest_House.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guest_House.Services.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly GuestHouseContext _context;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(GuestHouseContext context, ILogger<InvoiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync(string? status = null)
        {
            var query = _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Stay)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var cleanStatus = status.Trim().ToLowerInvariant();
                query = query.Where(i => i.InvoiceStatus == cleanStatus);
            }

            var invoices = await query
                .OrderByDescending(i => i.GeneratedAt)
                .ToListAsync();

            return invoices.Select(i => MapToResponseDto(i, i.InvoiceItems, i.Booking, _context));
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            if (invoiceId <= 0) return null;

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Stay)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            return invoice == null ? null : MapToResponseDto(invoice, invoice.InvoiceItems, invoice.Booking, _context);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByBookingIdAsync(int bookingId)
        {
            if (bookingId <= 0) return null;

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Stay)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.BookingId == bookingId);

            return invoice == null ? null : MapToResponseDto(invoice, invoice.InvoiceItems, invoice.Booking, _context);
        }

        /// <summary>
        /// Automatic invoice generation workflow for a booking
        /// </summary>
        public async Task<BillingResult<InvoiceResponseDto>> GenerateInvoiceAsync(GenerateInvoiceRequestDto request)
        {
            // 1. Validate booking ID
            if (request.BookingId <= 0)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    "A valid positive Booking ID is required.",
                    StatusCodes.Status400BadRequest);
            }

            // 2. Verify the booking actually exists with its relationships
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Include(b => b.Guest)
                .Include(b => b.Stay)
                    .ThenInclude(s => s!.ExpenseCharges)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    $"Booking with ID {request.BookingId} was not found.",
                    StatusCodes.Status404NotFound);
            }

            // Prevent duplicate invoice at application level
            var duplicate = await _context.Invoices.AnyAsync(i => i.BookingId == request.BookingId);
            if (duplicate)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    $"An invoice has already been generated for Booking ID {request.BookingId}. Duplicate invoices are prohibited.",
                    StatusCodes.Status409Conflict);
            }

            // 3 & 4. Find the correct booking_room records and room(s)
            var bookingRooms = booking.BookingRooms?.ToList() ?? new List<BookingRoom>();
            if (bookingRooms.Count == 0)
            {
                bookingRooms = await _context.BookingRooms
                    .Include(br => br.Room)
                    .Where(br => br.BookingId == booking.BookingId)
                    .ToListAsync();
            }

            if (bookingRooms.Count == 0)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    $"Booking ID {request.BookingId} has no assigned rooms to compute room charges.",
                    StatusCodes.Status400BadRequest);
            }

            // Validate monetary input parameters
            var errors = new List<string>();
            if (request.TaxAmount < 0) errors.Add("Tax amount cannot be negative.");
            if (request.DiscountAmount < 0) errors.Add("Discount amount cannot be negative.");
            if (request.PaidAmount < 0) errors.Add("Paid amount cannot be negative.");

            if (errors.Count > 0)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    "Invoice generation parameters failed validation.",
                    StatusCodes.Status400BadRequest,
                    errors);
            }

            // 5. Determine the correct stay
            var stay = booking.Stay ?? await _context.Stays
                .Include(s => s.ExpenseCharges)
                .FirstOrDefaultAsync(s => s.BookingId == booking.BookingId);

            // 6. Determine actual stay duration using existing project business rules
            var checkIn = stay?.ActualCheckin ?? booking.CheckInDate;
            var checkOut = stay?.ActualCheckout ?? booking.ExpectedCheckout;
            var durationNights = (checkOut.Date - checkIn.Date).Days;
            if (durationNights <= 0) durationNights = 1; // Minimum 1 billable night

            // 7 & 8. Retrieve stored booking room price and calculate room charges
            decimal roomChargeTotal = 0m;
            var roomInvoiceItems = new List<InvoiceItem>();

            foreach (var br in bookingRooms)
            {
                var roomPrice = br.RoomPrice;
                var roomAmount = roomPrice * durationNights;
                roomChargeTotal += roomAmount;

                var roomLabel = br.Room != null ? br.Room.RoomNumber : br.RoomId.ToString();
                roomInvoiceItems.Add(new InvoiceItem
                {
                    ItemType = "room",
                    Description = $"Room {roomLabel} ({durationNights} night(s) @ {roomPrice:N2}/night)",
                    Quantity = durationNights,
                    UnitPrice = roomPrice,
                    Amount = roomAmount
                });
            }

            // 9 & 10. Find all applicable expense_charge records for the stay and calculate extra charges
            decimal extraChargeTotal = 0m;
            var extraInvoiceItems = new List<InvoiceItem>();

            if (stay != null)
            {
                var charges = stay.ExpenseCharges?.ToList() ?? await _context.ExpenseCharges
                    .Where(c => c.StayId == stay.StayId)
                    .ToListAsync();

                foreach (var charge in charges)
                {
                    extraChargeTotal += charge.Amount;
                    extraInvoiceItems.Add(new InvoiceItem
                    {
                        ItemType = BillingConstants.MapChargeTypeToItemType(charge.ChargeType),
                        Description = !string.IsNullOrWhiteSpace(charge.Description)
                            ? charge.Description
                            : $"{charge.ChargeType.Replace('_', ' ')} charge",
                        Quantity = 1,
                        UnitPrice = charge.Amount,
                        Amount = charge.Amount
                    });
                }
            }

            // 11 & 12, 13 & 14. Apply tax_amount and discount_amount (without inventing arbitrary rates)
            var taxAmount = request.TaxAmount ?? 0.00m;
            var discountAmount = request.DiscountAmount ?? 0.00m;

            // 15. Calculate grand_total
            var grandTotal = roomChargeTotal + extraChargeTotal + taxAmount - discountAmount;
            if (grandTotal < 0) grandTotal = 0.00m;

            // 16. Determine paid_amount using the existing Payment integration contract
            var completedPayments = await _context.Payments
                .Where(p => p.BookingId == booking.BookingId &&
                           (p.PaymentStatus == "completed" || p.PaymentStatus == "successful" || p.PaymentStatus == "paid"))
                .ToListAsync();

            decimal paidFromPayments = completedPayments.Sum(p => p.Amount);
            decimal paidAmount = request.PaidAmount.HasValue && request.PaidAmount.Value > 0
                ? Math.Max(request.PaidAmount.Value, paidFromPayments)
                : paidFromPayments;

            // 17. Calculate due_amount
            var dueAmount = grandTotal - paidAmount;
            if (dueAmount < 0) dueAmount = 0.00m;

            // 18. Determine invoice_status
            string status;
            if (paidAmount >= grandTotal && grandTotal > 0)
            {
                status = "paid";
            }
            else if (paidAmount > 0)
            {
                status = "partially_paid";
            }
            else
            {
                status = "unpaid";
            }

            // Generate unique invoice number
            var invoiceNumber = GenerateInvoiceNumber(booking.BookingId);

            // Execute entire creation in a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 19 & 20. Create and save the invoice
                var invoice = new Invoice
                {
                    BookingId = booking.BookingId,
                    InvoiceNumber = invoiceNumber,
                    RoomChargeTotal = roomChargeTotal,
                    ExtraChargeTotal = extraChargeTotal,
                    TaxAmount = taxAmount,
                    DiscountAmount = discountAmount,
                    GrandTotal = grandTotal,
                    PaidAmount = paidAmount,
                    DueAmount = dueAmount,
                    InvoiceStatus = status,
                    GeneratedAt = DateTime.UtcNow
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // 21. Obtain real database-generated invoice ID
                var generatedInvoiceId = invoice.InvoiceId;

                // 22 & 23. Create and save invoice_item records using the real generated invoice ID
                var allItems = new List<InvoiceItem>();
                allItems.AddRange(roomInvoiceItems);
                allItems.AddRange(extraInvoiceItems);

                foreach (var item in allItems)
                {
                    item.InvoiceId = generatedInvoiceId;
                    _context.InvoiceItems.Add(item);
                }

                // Link completed payments to this generated invoice
                foreach (var payment in completedPayments.Where(p => p.InvoiceId == null))
                {
                    payment.InvoiceId = generatedInvoiceId;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Successfully generated and committed Invoice {InvoiceNumber} (ID: {InvoiceId}) for Booking {BookingId}. Grand Total: {GrandTotal}",
                    invoice.InvoiceNumber, generatedInvoiceId, booking.BookingId, invoice.GrandTotal);

                // 24. Retrieve the saved invoice from the database
                var persistedInvoice = await _context.Invoices
                    .Include(i => i.InvoiceItems)
                    .Include(i => i.Booking)
                        .ThenInclude(b => b.Guest)
                    .Include(i => i.Booking)
                        .ThenInclude(b => b.Stay)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.InvoiceId == generatedInvoiceId);

                if (persistedInvoice == null)
                {
                    return BillingResult<InvoiceResponseDto>.Failure(
                        "Invoice committed but failed to re-query from the database.",
                        StatusCodes.Status500InternalServerError);
                }

                // 25. Return the actual persisted invoice and its actual invoice items
                return BillingResult<InvoiceResponseDto>.Success(
                    MapToResponseDto(persistedInvoice, persistedInvoice.InvoiceItems, persistedInvoice.Booking, _context),
                    "Invoice generated successfully.",
                    StatusCodes.Status201Created);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "Database constraint violation while generating invoice for Booking {BookingId}", request.BookingId);

                if (dbEx.InnerException?.Message.Contains("UQ_invoice_booking") == true ||
                    await _context.Invoices.AnyAsync(i => i.BookingId == request.BookingId))
                {
                    return BillingResult<InvoiceResponseDto>.Failure(
                        $"An invoice has already been generated for Booking ID {request.BookingId}. Duplicate invoices are prohibited.",
                        StatusCodes.Status409Conflict);
                }

                return BillingResult<InvoiceResponseDto>.Failure(
                    "Database constraint violation occurred during invoice generation. No changes were committed.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction rolled back during invoice generation for Booking {BookingId}", request.BookingId);
                return BillingResult<InvoiceResponseDto>.Failure(
                    "An unexpected error occurred during invoice generation. No changes were committed.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message });
            }
        }

        public async Task<BillingResult<InvoiceResponseDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateDto dto)
        {
            if (invoiceId <= 0)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    "A valid positive Invoice ID is required.",
                    StatusCodes.Status400BadRequest);
            }

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Stay)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    $"Invoice with ID {invoiceId} was not found.",
                    StatusCodes.Status404NotFound);
            }

            var errors = new List<string>();

            if (dto.TaxAmount.HasValue && dto.TaxAmount.Value < 0)
                errors.Add("Tax amount cannot be negative.");

            if (dto.DiscountAmount.HasValue && dto.DiscountAmount.Value < 0)
                errors.Add("Discount amount cannot be negative.");

            if (dto.PaidAmount.HasValue && dto.PaidAmount.Value < 0)
                errors.Add("Paid amount cannot be negative.");

            if (!string.IsNullOrWhiteSpace(dto.InvoiceStatus) && !BillingConstants.IsValidInvoiceStatus(dto.InvoiceStatus))
            {
                errors.Add($"Invalid invoice_status '{dto.InvoiceStatus}'. Allowed values: {string.Join(", ", BillingConstants.AllowedInvoiceStatuses)}.");
            }

            if (errors.Count > 0)
            {
                return BillingResult<InvoiceResponseDto>.Failure(
                    "Invoice update validation failed.",
                    StatusCodes.Status400BadRequest,
                    errors);
            }

            if (dto.TaxAmount.HasValue)
                invoice.TaxAmount = dto.TaxAmount.Value;

            if (dto.DiscountAmount.HasValue)
                invoice.DiscountAmount = dto.DiscountAmount.Value;

            if (dto.PaidAmount.HasValue)
                invoice.PaidAmount = dto.PaidAmount.Value;

            // Recalculate totals
            invoice.GrandTotal = invoice.RoomChargeTotal + invoice.ExtraChargeTotal + invoice.TaxAmount - invoice.DiscountAmount;
            if (invoice.GrandTotal < 0) invoice.GrandTotal = 0.00m;

            invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
            if (invoice.DueAmount < 0) invoice.DueAmount = 0.00m;

            // Update status
            if (!string.IsNullOrWhiteSpace(dto.InvoiceStatus))
            {
                invoice.InvoiceStatus = dto.InvoiceStatus.Trim().ToLowerInvariant();
            }
            else if (invoice.InvoiceStatus != "cancelled")
            {
                if (invoice.PaidAmount >= invoice.GrandTotal && invoice.GrandTotal > 0)
                    invoice.InvoiceStatus = "paid";
                else if (invoice.PaidAmount > 0)
                    invoice.InvoiceStatus = "partially_paid";
                else
                    invoice.InvoiceStatus = "unpaid";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated invoice {InvoiceId}. Status: {Status}, Grand Total: {GrandTotal}, Due: {DueAmount}",
                invoice.InvoiceId, invoice.InvoiceStatus, invoice.GrandTotal, invoice.DueAmount);

            // Re-query from database to guarantee persisted values
            var persistedInvoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Stay)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            return BillingResult<InvoiceResponseDto>.Success(
                MapToResponseDto(persistedInvoice ?? invoice, (persistedInvoice ?? invoice).InvoiceItems, (persistedInvoice ?? invoice).Booking, _context),
                "Invoice updated successfully.");
        }

        public async Task<IEnumerable<InvoiceItemResponseDto>> GetInvoiceItemsAsync(int? invoiceId = null)
        {
            var query = _context.InvoiceItems.AsNoTracking();

            if (invoiceId.HasValue)
            {
                query = query.Where(item => item.InvoiceId == invoiceId.Value);
            }

            var items = await query.ToListAsync();
            return items.Select(MapItemToDto);
        }

        public async Task<InvoiceItemResponseDto?> GetInvoiceItemByIdAsync(int itemId)
        {
            if (itemId <= 0) return null;

            var item = await _context.InvoiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceItemId == itemId);

            return item == null ? null : MapItemToDto(item);
        }

        public async Task<BillingResult<InvoiceItemResponseDto>> AddInvoiceItemAsync(InvoiceItemCreateDto dto)
        {
            if (dto.InvoiceId <= 0)
            {
                return BillingResult<InvoiceItemResponseDto>.Failure(
                    "A valid positive Invoice ID is required.",
                    StatusCodes.Status400BadRequest);
            }

            var errors = new List<string>();

            // 1. Validate invoice existence
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice == null)
            {
                return BillingResult<InvoiceItemResponseDto>.Failure(
                    $"Invoice with ID {dto.InvoiceId} does not exist.",
                    StatusCodes.Status404NotFound);
            }

            if (invoice.InvoiceStatus == "paid" || invoice.InvoiceStatus == "cancelled")
            {
                return BillingResult<InvoiceItemResponseDto>.Failure(
                    $"Cannot add items to an invoice that is already {invoice.InvoiceStatus}.",
                    StatusCodes.Status400BadRequest);
            }

            // 2. Validate ItemType
            if (!BillingConstants.IsValidInvoiceItemType(dto.ItemType))
            {
                errors.Add($"Invalid item_type '{dto.ItemType}'. Allowed values: {string.Join(", ", BillingConstants.AllowedInvoiceItemTypes)}.");
            }

            // 3. Validate Quantity & UnitPrice
            if (dto.Quantity <= 0) errors.Add("Quantity must be at least 1.");
            if (dto.UnitPrice < 0) errors.Add("UnitPrice cannot be negative.");

            decimal computedAmount = dto.Amount ?? (dto.Quantity * dto.UnitPrice);
            if (computedAmount < 0) errors.Add("Amount cannot be negative.");

            if (errors.Count > 0)
            {
                return BillingResult<InvoiceItemResponseDto>.Failure(
                    "Invoice item validation failed.",
                    StatusCodes.Status400BadRequest,
                    errors);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = dto.InvoiceId,
                    ItemType = dto.ItemType.Trim().ToLowerInvariant(),
                    Description = dto.Description.Trim(),
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice,
                    Amount = computedAmount
                };

                _context.InvoiceItems.Add(invoiceItem);

                // Update invoice totals
                if (invoiceItem.ItemType == "room")
                {
                    invoice.RoomChargeTotal += invoiceItem.Amount;
                }
                else
                {
                    invoice.ExtraChargeTotal += invoiceItem.Amount;
                }

                invoice.GrandTotal = invoice.RoomChargeTotal + invoice.ExtraChargeTotal + invoice.TaxAmount - invoice.DiscountAmount;
                if (invoice.GrandTotal < 0) invoice.GrandTotal = 0.00m;

                invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
                if (invoice.DueAmount < 0) invoice.DueAmount = 0.00m;

                if (invoice.PaidAmount >= invoice.GrandTotal && invoice.GrandTotal > 0)
                    invoice.InvoiceStatus = "paid";
                else if (invoice.PaidAmount > 0)
                    invoice.InvoiceStatus = "partially_paid";
                else
                    invoice.InvoiceStatus = "unpaid";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Added invoice item {ItemId} to invoice {InvoiceId}", invoiceItem.InvoiceItemId, invoice.InvoiceId);

                // Re-query saved item to verify persistence
                var persistedItem = await _context.InvoiceItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.InvoiceItemId == invoiceItem.InvoiceItemId);

                return BillingResult<InvoiceItemResponseDto>.Success(
                    MapItemToDto(persistedItem ?? invoiceItem),
                    "Invoice item added and invoice totals updated successfully.",
                    StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to add invoice item to invoice {InvoiceId}", dto.InvoiceId);
                return BillingResult<InvoiceItemResponseDto>.Failure(
                    "Database transaction failed while adding invoice item.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message });
            }
        }

        private static string GenerateInvoiceNumber(int bookingId)
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomSuffix = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            return $"INV-{dateStr}-{bookingId:D4}-{randomSuffix}";
        }

        private static InvoiceResponseDto MapToResponseDto(
            Invoice i,
            IEnumerable<InvoiceItem> invoiceItems,
            Booking? booking,
            GuestHouseContext? context)
        {
            int? stayId = booking?.Stay?.StayId;
            if (!stayId.HasValue && booking != null && context != null)
            {
                stayId = context.Stays
                    .Where(s => s.BookingId == booking.BookingId)
                    .Select(s => (int?)s.StayId)
                    .FirstOrDefault();
            }

            return new InvoiceResponseDto
            {
                InvoiceId = i.InvoiceId,
                BookingId = i.BookingId,
                InvoiceNumber = i.InvoiceNumber,
                BookingReference = booking?.BookingReference,
                GuestName = booking?.Guest?.FullName,
                StayId = stayId,
                RoomChargeTotal = i.RoomChargeTotal,
                ExtraChargeTotal = i.ExtraChargeTotal,
                TaxAmount = i.TaxAmount,
                DiscountAmount = i.DiscountAmount,
                GrandTotal = i.GrandTotal,
                PaidAmount = i.PaidAmount,
                DueAmount = i.DueAmount,
                InvoiceStatus = i.InvoiceStatus,
                GeneratedAt = i.GeneratedAt,
                Items = invoiceItems?.Select(MapItemToDto).ToList() ?? new List<InvoiceItemResponseDto>()
            };
        }

        private static InvoiceItemResponseDto MapItemToDto(InvoiceItem item)
        {
            return new InvoiceItemResponseDto
            {
                InvoiceItemId = item.InvoiceItemId,
                InvoiceId = item.InvoiceId,
                ItemType = item.ItemType,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Amount = item.Amount
            };
        }
    }
}
