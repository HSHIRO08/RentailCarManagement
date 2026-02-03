using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class ChatSession
{
    public Guid ChatSessionId { get; set; }

    public Guid UserId { get; set; }

    public string? Subject { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
