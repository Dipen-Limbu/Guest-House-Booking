using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("room_category")]
[Index("HotelId", "CategoryName", Name = "UQ_room_category_hotel_name", IsUnique = true)]
public partial class RoomCategory
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("category_name")]
    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [Column("base_price", TypeName = "decimal(10, 2)")]
    public decimal BasePrice { get; set; }

    [Column("max_occupancy")]
    public int MaxOccupancy { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("RoomCategories")]
    public virtual Hotel Hotel { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
