using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Customer
{
    public Guid CustomerId { get; set; }

    public Guid UserId { get; set; }

    public string? DriverLicenseNumber { get; set; }

    public DateOnly? LicenseExpiryDate { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual AspNetUser User { get; set; } = null!;
}
