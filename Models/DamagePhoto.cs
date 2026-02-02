using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentailCarManagement.Models;

/// <summary>
/// ?nh h? h?ng
/// </summary>
public class DamagePhoto
{
    [Key]
    public Guid PhotoId { get; set; }

    /// <summary>
    /// ID báo cáo h? h?ng
    /// </summary>
    public Guid DamageReportId { get; set; }

    /// <summary>
    /// URL ?nh
    /// </summary>
    [Required]
    public string PhotoUrl { get; set; } = null!;

    /// <summary>
    /// Mô t?
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Th? t? hi?n th?
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    public DateTime? CreatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("DamageReportId")]
    public virtual DamageReport DamageReport { get; set; } = null!;
}
