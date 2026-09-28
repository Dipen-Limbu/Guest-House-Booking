using System;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Hotel
{
    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? WebsiteUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class HotelCreateDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(255)]
        [Url]
        public string? WebsiteUrl { get; set; }
    }

    public class HotelUpdateDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(255)]
        [Url]
        public string? WebsiteUrl { get; set; }
    }
}
