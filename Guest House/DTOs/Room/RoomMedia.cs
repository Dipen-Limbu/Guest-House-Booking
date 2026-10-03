using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Room
{
    public class CreateRoomMediaDto      // RoomId comes from the URL, not the body
    {
        [Required, StringLength(10)] public string MediaType { get; set; } = string.Empty;  // "image" | "video"
        [Required, StringLength(500)] public string FileUrl { get; set; } = string.Empty;
        [StringLength(150)] public string? Caption { get; set; }
        [Range(0, int.MaxValue)] public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateRoomMediaDto
    {
        [Required(ErrorMessage = "Media type is required.")]
        [StringLength(10, ErrorMessage = "Media type cannot exceed 10 characters.")]
        public string MediaType { get; set; } = string.Empty;

        [Required(ErrorMessage = "File URL is required.")]
        [StringLength(500, ErrorMessage = "File URL cannot exceed 500 characters.")]
        public string FileUrl { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Caption cannot exceed 150 characters.")]
        public string? Caption { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order cannot be negative.")]
        public int DisplayOrder { get; set; } = 0;
    }

    public class RoomMediaResponseDto
    {
        public int MediaId { get; set; }
        public int RoomId { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? UploadedAt { get; set; }
    }
}
