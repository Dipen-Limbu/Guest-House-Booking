using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Room
{
    public class CreateRoomDto
    {
        [Required, Range(1, int.MaxValue)] public int HotelId { get; set; }
        [Required, Range(1, int.MaxValue)] public int CategoryId { get; set; }
        [Required, StringLength(20)] public string RoomNumber { get; set; } = string.Empty;
        [Range(-5, 200)] public int? FloorNumber { get; set; }
        [StringLength(20)] public string? Status { get; set; }   // optional, defaults to "available"
        public string? Description { get; set; }
    }

    public class UpdateRoomDto   // no HotelId, no Status
    {
        [Required, Range(1, int.MaxValue)] public int CategoryId { get; set; }
        [Required, StringLength(20)] public string RoomNumber { get; set; } = string.Empty;
        [Range(-5, 200)] public int? FloorNumber { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateRoomStatusDto
    {
        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;   // available | occupied | maintenance | cleaning
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;   // from Category
        public decimal BasePrice { get; set; }                     // from Category (this is the "pricing")
        public int MaxOccupancy { get; set; }                      // from Category
        public string RoomNumber { get; set; } = string.Empty;
        public int? FloorNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<RoomMediaResponseDto> Media { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
