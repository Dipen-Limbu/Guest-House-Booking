using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("staff_user")]
[Index("Username", Name = "UQ_staff_user_username", IsUnique = true)]
public partial class StaffUser
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("full_name")]
    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("StaffUsers")]
    public virtual Hotel Hotel { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("StaffUsers")]
    public virtual Role Role { get; set; } = null!;
}
