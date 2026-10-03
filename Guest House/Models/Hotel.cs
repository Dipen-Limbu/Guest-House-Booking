using System;
using System.Collections.Generic;

namespace Guest_House.Models;

public partial class Hotel
{
    public int HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? WebsiteUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<HotelExpense> HotelExpenses { get; set; } = new List<HotelExpense>();

    public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();

    public virtual ICollection<RoomCategory> RoomCategories { get; set; } = new List<RoomCategory>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<StaffUser> StaffUsers { get; set; } = new List<StaffUser>();

    public virtual ICollection<WebsiteSyncLog> WebsiteSyncLogs { get; set; } = new List<WebsiteSyncLog>();
}
