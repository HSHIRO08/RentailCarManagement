using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Supplier
{
    public Guid SupplierId { get; set; }

    public Guid UserId { get; set; }

    public string? CompanyName { get; set; }

    public string? TaxCode { get; set; }

    public string? Address { get; set; }

    public string? BankAccount { get; set; }

    public string? BankName { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

    public virtual ApplicationUser User { get; set; } = null!;
}
