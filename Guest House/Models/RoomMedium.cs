using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class RoomMedium
{
    public int MediaId { get; set; }

    public int RoomId { get; set; }

    public string MediaType { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Room Room { get; set; } = null!;
}
