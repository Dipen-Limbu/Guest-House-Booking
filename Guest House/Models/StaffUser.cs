using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class StaffUser
{
    public int UserId { get; set; }

    public int HotelId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
