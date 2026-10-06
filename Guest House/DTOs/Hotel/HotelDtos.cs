using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Hotel
{
    // ==========================================
    // HOTEL DTOs
    // ==========================================

    public class CreateHotelDto
    {
        [Required(ErrorMessage = "Hotel name is required.")]
        [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid website URL format.")]
        [StringLength(255, ErrorMessage = "Website URL cannot exceed 255 characters.")]
        public string? WebsiteUrl { get; set; }
    }

    public class UpdateHotelDto
    {
        [Required(ErrorMessage = "Hotel name is required.")]
        [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid website URL format.")]
        [StringLength(255, ErrorMessage = "Website URL cannot exceed 255 characters.")]
        public string? WebsiteUrl { get; set; }
    }

    public class HotelResponseDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? WebsiteUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalRooms { get; set; }
        public int TotalStaff { get; set; }
    }

    // ==========================================
    // HOTEL EXPENSE DTOs
    // ==========================================

    public class CreateHotelExpenseDto
    {
        [Required(ErrorMessage = "Hotel ID is required.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Expense category is required.")]
        [StringLength(100, ErrorMessage = "Expense category cannot exceed 100 characters.")]
        public string ExpenseCategory { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [StringLength(255, ErrorMessage = "Receipt URL cannot exceed 255 characters.")]
        public string? ReceiptUrl { get; set; }
    }

    public class UpdateHotelExpenseDto
    {
        [StringLength(100, ErrorMessage = "Expense category cannot exceed 100 characters.")]
        public string? ExpenseCategory { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal? Amount { get; set; }

        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [StringLength(255, ErrorMessage = "Receipt URL cannot exceed 255 characters.")]
        public string? ReceiptUrl { get; set; }
    }

    public class HotelExpenseResponseDto
    {
        public int ExpenseId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public string ExpenseCategory { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? ReceiptUrl { get; set; }
    }

    public class HotelExpenseSummaryDto
    {
        public int HotelId { get; set; }
        public decimal TotalExpenses { get; set; }
        public int ExpenseCount { get; set; }
        public List<HotelExpenseResponseDto> Expenses { get; set; } = new();
    }

    // ==========================================
    // WEBSITE SYNC LOG DTOs
    // ==========================================

    public class CreateWebsiteSyncLogDto
    {
        [Required(ErrorMessage = "Hotel ID is required.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Entity type is required.")]
        [StringLength(50, ErrorMessage = "Entity type cannot exceed 50 characters.")]
        public string EntityType { get; set; } = null!;

        [Required(ErrorMessage = "Entity ID is required.")]
        public int EntityId { get; set; }

        [Required(ErrorMessage = "Sync status is required.")]
        [StringLength(50, ErrorMessage = "Sync status cannot exceed 50 characters.")]
        public string SyncStatus { get; set; } = null!;

        public string? ErrorMessage { get; set; }
    }

    public class WebsiteSyncLogResponseDto
    {
        public int SyncId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public int EntityId { get; set; }
        public string SyncStatus { get; set; } = null!;
        public string? ErrorMessage { get; set; }
        public DateTime SyncedAt { get; set; }
    }
}
