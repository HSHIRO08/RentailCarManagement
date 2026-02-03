using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Complaint
{
    public Guid ComplaintId { get; set; }

    public Guid RentalId { get; set; }

    public Guid UserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Resolution { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual Rental Rental { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
