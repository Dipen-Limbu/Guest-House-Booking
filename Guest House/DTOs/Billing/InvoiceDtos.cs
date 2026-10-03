using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Billing
{
    /// <summary>
    /// Parameters required to automatically generate an invoice for a booking
    /// </summary>
    public class GenerateInvoiceRequestDto
    {
        /// <summary>
        /// ID of the target booking to invoice
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "BookingId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid positive BookingId is required.")]
        public int BookingId { get; set; } = 1;

        /// <summary>
        /// Optional explicit tax amount (e.g. 0.00). Do not invent tax percentages.
        /// </summary>
        /// <example>0.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Tax amount cannot be negative.")]
        public decimal? TaxAmount { get; set; } = 0.00m;

        /// <summary>
        /// Optional explicit discount amount deducted from total (e.g. 0.00)
        /// </summary>
        /// <example>0.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Discount amount cannot be negative.")]
        public decimal? DiscountAmount { get; set; } = 0.00m;

        /// <summary>
        /// Optional upfront or advance payment recorded at billing time
        /// </summary>
        /// <example>0.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Paid amount cannot be negative.")]
        public decimal? PaidAmount { get; set; } = 0.00m;
    }

    /// <summary>
    /// Financial and status update parameters for an existing invoice
    /// </summary>
    public class InvoiceUpdateDto
    {
        /// <summary>
        /// Updated tax amount
        /// </summary>
        /// <example>0.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Tax amount cannot be negative.")]
        public decimal? TaxAmount { get; set; }

        /// <summary>
        /// Updated discount amount
        /// </summary>
        /// <example>100.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Discount amount cannot be negative.")]
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Updated cumulative paid amount (compatible with Payment module)
        /// </summary>
        /// <example>9000.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Paid amount cannot be negative.")]
        public decimal? PaidAmount { get; set; }

        /// <summary>
        /// Explicit status override: 'unpaid', 'partially_paid', 'paid', 'cancelled'
        /// </summary>
        /// <example>paid</example>
        [StringLength(20, ErrorMessage = "InvoiceStatus cannot exceed 20 characters.")]
        public string? InvoiceStatus { get; set; }
    }

    /// <summary>
    /// Persisted invoice details including customer charges, breakdown items, and current balances
    /// </summary>
    public class InvoiceResponseDto
    {
        /// <summary>
        /// Database-generated unique invoice ID (identity primary key)
        /// </summary>
        /// <example>1</example>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Associated booking ID
        /// </summary>
        /// <example>1</example>
        public int BookingId { get; set; }

        /// <summary>
        /// Unique system invoice reference number
        /// </summary>
        /// <example>INV-20261003-0001-A1B2</example>
        public string InvoiceNumber { get; set; } = null!;

        /// <summary>
        /// Booking reference code from booking table
        /// </summary>
        /// <example>BKG-2026-001</example>
        public string? BookingReference { get; set; }

        /// <summary>
        /// Full name of the primary registered guest
        /// </summary>
        /// <example>Ramesh Sharma</example>
        public string? GuestName { get; set; }

        /// <summary>
        /// Stay ID linked to this booking
        /// </summary>
        /// <example>1</example>
        public int? StayId { get; set; }

        /// <summary>
        /// Total room charges calculated from booked rooms and nights
        /// </summary>
        /// <example>7500.00</example>
        public decimal RoomChargeTotal { get; set; }

        /// <summary>
        /// Total incidental extra charges (laundry, room order, minibar, etc.)
        /// </summary>
        /// <example>1200.00</example>
        public decimal ExtraChargeTotal { get; set; }

        /// <summary>
        /// Applied tax amount
        /// </summary>
        /// <example>0.00</example>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Applied discount amount
        /// </summary>
        /// <example>200.00</example>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Grand total: RoomChargeTotal + ExtraChargeTotal + TaxAmount - DiscountAmount
        /// </summary>
        /// <example>8500.00</example>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// Total recorded payment toward this invoice
        /// </summary>
        /// <example>2000.00</example>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Outstanding due amount: max(0, GrandTotal - PaidAmount)
        /// </summary>
        /// <example>6500.00</example>
        public decimal DueAmount { get; set; }

        /// <summary>
        /// Current payment status: 'unpaid', 'partially_paid', 'paid', 'cancelled'
        /// </summary>
        /// <example>partially_paid</example>
        public string InvoiceStatus { get; set; } = null!;

        /// <summary>
        /// Generation UTC timestamp
        /// </summary>
        /// <example>2026-10-03T12:00:00Z</example>
        public DateTime? GeneratedAt { get; set; }

        /// <summary>
        /// Itemized line-item charges
        /// </summary>
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }
}
