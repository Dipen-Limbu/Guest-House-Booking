using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("website_sync_log")]
public partial class WebsiteSyncLog
{
    [Key]
    [Column("sync_id")]
    public int SyncId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("entity_type")]
    [StringLength(20)]
    public string EntityType { get; set; } = null!;

    [Column("entity_id")]
    public int EntityId { get; set; }

    [Column("sync_status")]
    [StringLength(20)]
    public string SyncStatus { get; set; } = null!;

    [Column("error_message")]
    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    [Column("synced_at")]
    public DateTime? SyncedAt { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("WebsiteSyncLogs")]
    public virtual Hotel Hotel { get; set; } = null!;
}
