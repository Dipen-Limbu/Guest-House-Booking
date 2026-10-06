using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Billing
{
    /// <summary>
    /// Payload required to record a customer incidental expense charge
    /// </summary>
    public class ExpenseChargeCreateDto
    {
        /// <summary>
        /// Target stay ID where charge was incurred
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "StayId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid positive StayId is required.")]
        public int StayId { get; set; } = 1;

        /// <summary>
        /// Category: 'room_charge', 'room_order', 'minibar', 'laundry', 'damage', 'extra_bed', 'service_fee', 'other'
        /// </summary>
        /// <example>laundry</example>
        [Required(ErrorMessage = "ChargeType is required.")]
        [StringLength(30, ErrorMessage = "ChargeType cannot exceed 30 characters.")]
        public string ChargeType { get; set; } = "laundry";

        /// <summary>
        /// Description of the service or item provided
        /// </summary>
        /// <example>2x Shirts laundry and dry cleaning</example>
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional room order ID if this charge originates from room service
        /// </summary>
        /// <example>1</example>
        public int? RoomOrderId { get; set; }

        /// <summary>
        /// Monetary amount charged to the guest (must be positive)
        /// </summary>
        /// <example>350.00</example>
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be a positive value greater than 0.")]
        public decimal Amount { get; set; } = 350.00m;

        /// <summary>
        /// UTC timestamp when charge was incurred (defaults to current time if omitted)
        /// </summary>
        /// <example>2026-10-03T12:00:00Z</example>
        public DateTime? IncurredAt { get; set; }
    }

    /// <summary>
    /// Payload to update an existing customer expense charge
    /// </summary>
    public class ExpenseChargeUpdateDto
    {
        /// <summary>
        /// Updated category: 'room_charge', 'room_order', 'minibar', 'laundry', 'damage', 'extra_bed', 'service_fee', 'other'
        /// </summary>
        /// <example>minibar</example>
        [StringLength(30, ErrorMessage = "ChargeType cannot exceed 30 characters.")]
        public string? ChargeType { get; set; }

        /// <summary>
        /// Updated description
        /// </summary>
        /// <example>Imported chocolates and juices</example>
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional room order ID link
        /// </summary>
        /// <example>1</example>
        public int? RoomOrderId { get; set; }

        /// <summary>
        /// Updated charge amount
        /// </summary>
        /// <example>450.00</example>
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be a positive value greater than 0.")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// Updated incurred timestamp
        /// </summary>
        /// <example>2026-10-03T12:00:00Z</example>
        public DateTime? IncurredAt { get; set; }
    }

    /// <summary>
    /// Persisted customer expense charge details
    /// </summary>
    public class ExpenseChargeResponseDto
    {
        /// <summary>
        /// Database-generated charge primary key
        /// </summary>
        /// <example>1</example>
        public int ChargeId { get; set; }

        /// <summary>
        /// Stay ID where charge occurred
        /// </summary>
        /// <example>1</example>
        public int StayId { get; set; }

        /// <summary>
        /// Charge category
        /// </summary>
        /// <example>laundry</example>
        public string ChargeType { get; set; } = null!;

        /// <summary>
        /// Item or service description
        /// </summary>
        /// <example>2x Shirts laundry and dry cleaning</example>
        public string? Description { get; set; }

        /// <summary>
        /// Linked room order ID if applicable
        /// </summary>
        /// <example>1</example>
        public int? RoomOrderId { get; set; }

        /// <summary>
        /// Order reference number if linked
        /// </summary>
        /// <example>ORD-2026-001</example>
        public string? RoomOrderNumber { get; set; }

        /// <summary>
        /// Charge amount
        /// </summary>
        /// <example>350.00</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// When the expense was incurred
        /// </summary>
        /// <example>2026-10-03T12:00:00Z</example>
        public DateTime? IncurredAt { get; set; }
    }
}
