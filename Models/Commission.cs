using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class Commission
{
    public Guid CommissionId { get; set; }

    public Guid RentalId { get; set; }

    public Guid SupplierId { get; set; }

    public decimal? CommissionRate { get; set; }

    public decimal SystemAmount { get; set; }

    public decimal SupplierAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Rental Rental { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
