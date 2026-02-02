using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Car
{
    public Guid CarId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid CategoryId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int? Year { get; set; }

    public int? Seats { get; set; }

    public string? FuelType { get; set; }

    public string? Transmission { get; set; }

    public decimal PricePerDay { get; set; }

    public decimal? PricePerHour { get; set; }

    public string? Status { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CarDocument> CarDocuments { get; set; } = new List<CarDocument>();

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    public virtual CarCategory Category { get; set; } = null!;

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual Supplier Supplier { get; set; } = null!;
}
