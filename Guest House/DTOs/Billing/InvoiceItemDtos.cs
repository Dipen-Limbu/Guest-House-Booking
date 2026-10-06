using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Billing
{
    /// <summary>
    /// Payload required to manually append an itemized line entry to an invoice
    /// </summary>
    public class InvoiceItemCreateDto
    {
        /// <summary>
        /// ID of the parent invoice
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "InvoiceId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid positive InvoiceId is required.")]
        public int InvoiceId { get; set; } = 1;

        /// <summary>
        /// Item category: 'room', 'room_order', 'laundry', 'minibar', 'damage', 'extra_bed', 'service', 'other'
        /// </summary>
        /// <example>service</example>
        [Required(ErrorMessage = "ItemType is required.")]
        [StringLength(30, ErrorMessage = "ItemType cannot exceed 30 characters.")]
        public string ItemType { get; set; } = "service";

        /// <summary>
        /// Description of the line item
        /// </summary>
        /// <example>Airport shuttle pickup service</example>
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Quantity or nights
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Rate per unit
        /// </summary>
        /// <example>500.00</example>
        [Required(ErrorMessage = "UnitPrice is required.")]
        [Range(0.00, 9999999.99, ErrorMessage = "UnitPrice cannot be negative.")]
        public decimal UnitPrice { get; set; } = 500.00m;

        /// <summary>
        /// Optional explicit line amount. If omitted, automatically computed as Quantity * UnitPrice.
        /// </summary>
        /// <example>500.00</example>
        [Range(0.00, 9999999.99, ErrorMessage = "Amount cannot be negative.")]
        public decimal? Amount { get; set; }
    }

    /// <summary>
    /// Persisted invoice line item details
    /// </summary>
    public class InvoiceItemResponseDto
    {
        /// <summary>
        /// Database-generated invoice item primary key
        /// </summary>
        /// <example>1</example>
        public int InvoiceItemId { get; set; }

        /// <summary>
        /// Parent invoice ID
        /// </summary>
        /// <example>1</example>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        /// <example>room</example>
        public string ItemType { get; set; } = null!;

        /// <summary>
        /// Item description
        /// </summary>
        /// <example>Room 101 (3 night(s) @ 2,500.00/night)</example>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Quantity
        /// </summary>
        /// <example>3</example>
        public int Quantity { get; set; }

        /// <summary>
        /// Price per unit
        /// </summary>
        /// <example>2500.00</example>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Total calculated amount
        /// </summary>
        /// <example>7500.00</example>
        public decimal Amount { get; set; }
    }
}
