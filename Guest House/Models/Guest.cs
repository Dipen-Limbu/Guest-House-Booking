using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Models;

[Table("guest")]
public partial class Guest
{
    [Key]
    [Column("guest_id")]
    public int GuestId { get; set; }

    [Column("full_name")]
    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [Column("email")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string? Address { get; set; }

    [Column("id_proof_type")]
    [StringLength(50)]
    public string? IdProofType { get; set; }

    [Column("id_proof_number")]
    [StringLength(50)]
    public string? IdProofNumber { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Guest")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
