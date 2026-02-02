using System;
using System.Collections.Generic;

namespace RentailCarManagement.Models;

public partial class ChatMessage
{
    public Guid MessageId { get; set; }

    public Guid ChatSessionId { get; set; }

    public string SenderType { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual ChatSession ChatSession { get; set; } = null!;
}
