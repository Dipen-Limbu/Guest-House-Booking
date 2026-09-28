using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.HotelExpense
{
    public class HotelExpenseDto
    {
        public int ExpenseId { get; set; }
        public int HotelId { get; set; }
        public string ExpenseCategory { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? ReceiptUrl { get; set; }
    }

    public class HotelExpenseCreateDto
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        [StringLength(100)]
        public string ExpenseCategory { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression("^(cash|card|bank_transfer|other)$", ErrorMessage = "Payment method must be cash, card, bank_transfer, or other.")]
        public string PaymentMethod { get; set; } = null!;

        public DateTime? ExpenseDate { get; set; }

        [StringLength(500)]
        public string? ReceiptUrl { get; set; }
    }

    public class HotelExpenseUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string ExpenseCategory { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression("^(cash|card|bank_transfer|other)$", ErrorMessage = "Payment method must be cash, card, bank_transfer, or other.")]
        public string PaymentMethod { get; set; } = null!;

        public DateTime? ExpenseDate { get; set; }

        [StringLength(500)]
        public string? ReceiptUrl { get; set; }
    }
}
