using System.ComponentModel.DataAnnotations;
using Guest_House.DTOs.Room;

namespace Guest_House.DTOs.Room
{
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Hotel id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hotel id must be a positive number.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Category id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Category id must be a positive number.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Range(-5, 200, ErrorMessage = "Floor number is out of range.")]
        public int? FloorNumber { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string? Status { get; set; }

        public string? Description { get; set; }
    }

    public class UpdateRoomDto
    {
        [Required(ErrorMessage = "Category id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Category id must be a positive number.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Range(-5, 200, ErrorMessage = "Floor number is out of range.")]
        public int? FloorNumber { get; set; }

        public string? Description { get; set; }
    }

    public class UpdateRoomStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; } = string.Empty;
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int? FloorNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<RoomMediaResponseDto> Media { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}