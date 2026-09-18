using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("hotel")]
public partial class Hotel
{
    [Key]
    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("name")]
    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("address")]
    [StringLength(255)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("email")]
    [StringLength(150)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("website_url")]
    [StringLength(255)]
    [Unicode(false)]
    public string? WebsiteUrl { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Hotel")]
    public virtual ICollection<HotelExpense> HotelExpenses { get; set; } = new List<HotelExpense>();

    [InverseProperty("Hotel")]
    public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();

    [InverseProperty("Hotel")]
    public virtual ICollection<RoomCategory> RoomCategories { get; set; } = new List<RoomCategory>();

    [InverseProperty("Hotel")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    [InverseProperty("Hotel")]
    public virtual ICollection<StaffUser> StaffUsers { get; set; } = new List<StaffUser>();

    [InverseProperty("Hotel")]
    public virtual ICollection<WebsiteSyncLog> WebsiteSyncLogs { get; set; } = new List<WebsiteSyncLog>();
}
