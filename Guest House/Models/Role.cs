using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<StaffUser> StaffUsers { get; set; } = new List<StaffUser>();
}
