using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.RoomCategory
{
    public class CreateRoomCategoryDto
    {
        [Required(ErrorMessage = "Hotel id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hotel id must be a positive number.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "Base price must be between 0 and 99,999,999.99.")]
        public decimal BasePrice { get; set; }

        [Range(1, 50, ErrorMessage = "Max occupancy must be between 1 and 50.")]
        public int MaxOccupancy { get; set; } = 1;

        public string? Description { get; set; }
    }

    public class UpdateRoomCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "Base price must be between 0 and 99,999,999.99.")]
        public decimal BasePrice { get; set; }

        [Range(1, 50, ErrorMessage = "Max occupancy must be between 1 and 50.")]
        public int MaxOccupancy { get; set; } = 1;

        public string? Description { get; set; }
    }

    public class RoomCategoryResponseDto
    {
        public int CategoryId { get; set; }
        public int HotelId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public string? Description { get; set; }
        public int RoomCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}