using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("room_media")]
public partial class RoomMedium
{
    [Key]
    [Column("media_id")]
    public int MediaId { get; set; }

    [Column("room_id")]
    public int RoomId { get; set; }

    [Column("media_type")]
    [StringLength(10)]
    public string MediaType { get; set; } = null!;

    [Column("file_url")]
    [StringLength(500)]
    public string FileUrl { get; set; } = null!;

    [Column("caption")]
    [StringLength(150)]
    public string? Caption { get; set; }

    [Column("display_order")]
    public int? DisplayOrder { get; set; }

    [Column("uploaded_at")]
    public DateTime? UploadedAt { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("RoomMedia")]
    public virtual Room Room { get; set; } = null!;
}
