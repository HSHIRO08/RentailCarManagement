using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid RentalId { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Rental Rental { get; set; } = null!;
}
