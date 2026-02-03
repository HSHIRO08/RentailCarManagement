using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Rental
{
    public Guid RentalId { get; set; }

    public Guid CarId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Commission? Commission { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Review? Review { get; set; }
    public string CarImages { get; internal set; }
}
