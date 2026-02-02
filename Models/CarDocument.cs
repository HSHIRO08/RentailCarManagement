using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class CarDocument
{
    public Guid DocumentId { get; set; }

    public Guid CarId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string? DocumentNumber { get; set; }

    public string? DocumentUrl { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
