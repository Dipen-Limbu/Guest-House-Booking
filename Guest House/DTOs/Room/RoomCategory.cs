using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Room
{
    public class CreateRoomCategoryDto
    {
        [Required, Range(1, int.MaxValue)]
        public int HotelId { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal BasePrice { get; set; }

        [Range(1, 50)]
        public int MaxOccupancy { get; set; } = 1;

        public string? Description { get; set; }
    }
    public class UpdateRoomCategoryDto   // same as Create but WITHOUT HotelId
    {
        [Required, StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal BasePrice { get; set; }
        [Range(1, 50)]
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
        public int RoomCount { get; set; }          // computed: how many rooms use this category
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

