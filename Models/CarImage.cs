using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class CarImage
{
    public Guid ImageId { get; set; }

    public Guid CarId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
