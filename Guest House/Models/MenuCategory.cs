using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("menu_category")]
[Index("HotelId", "CategoryName", Name = "UQ_menu_category_hotel_name", IsUnique = true)]
public partial class MenuCategory
{
    [Key]
    [Column("menu_category_id")]
    public int MenuCategoryId { get; set; }

    [Column("hotel_id")]
    public int HotelId { get; set; }

    [Column("category_name")]
    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("MenuCategories")]
    public virtual Hotel Hotel { get; set; } = null!;

    [InverseProperty("MenuCategory")]
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
