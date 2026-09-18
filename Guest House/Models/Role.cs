using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("role")]
[Index("RoleName", Name = "UQ_role_name", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("role_name")]
    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<StaffUser> StaffUsers { get; set; } = new List<StaffUser>();
}
