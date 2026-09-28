using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.WebsiteSyncLog
{
    public class WebsiteSyncLogDto
    {
        public int SyncId { get; set; }
        public int HotelId { get; set; }
        public string EntityType { get; set; } = null!;
        public int EntityId { get; set; }
        public string SyncStatus { get; set; } = null!;
        public string? ErrorMessage { get; set; }
        public DateTime? SyncedAt { get; set; }
    }

    public class WebsiteSyncLogCreateDto
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(room|category|availability|price)$", ErrorMessage = "Entity type must be room, category, availability, or price.")]
        public string EntityType { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "EntityId must be positive.")]
        public int EntityId { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(success|failed)$", ErrorMessage = "Sync status must be success or failed.")]
        public string SyncStatus { get; set; } = null!;

        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        public DateTime? SyncedAt { get; set; }
    }
}
