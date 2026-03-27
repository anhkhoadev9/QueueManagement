using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using System;

namespace QueueManagement.Domain.Entities
{
    public class TicketHistory : BaseEntity
    {
        public Guid QueueTicketId { get; private set; }
        public QueueTicket QueueTicket { get; private set; } = null!;
        
        public TicketStatus OldStatus { get; private set; }
        public TicketStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string? ChangedBy { get; private set; } // Tên/ID người thao tác (nếu có tài khoản)

        public TicketHistory() { }

        public TicketHistory(Guid queueTicketId, TicketStatus oldStatus, TicketStatus newStatus, string? changedBy = null)
        {
            QueueTicketId = queueTicketId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.UtcNow;
            ChangedBy = changedBy;
        }
    }
}
