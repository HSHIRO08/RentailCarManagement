using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public Guid RentalId { get; set; }

    public Guid CustomerId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Rental Rental { get; set; } = null!;
}
