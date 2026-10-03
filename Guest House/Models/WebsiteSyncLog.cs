using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class WebsiteSyncLog
{
    public int SyncId { get; set; }

    public int HotelId { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public string SyncStatus { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public DateTime SyncedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
}
