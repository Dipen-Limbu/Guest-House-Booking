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

    public class UpdateRoomMediaDto { /* identical fields to CreateRoomMediaDto */ }

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
