using QueueManagement.Domain.Enum;
using System;

namespace QueueManagement.Application.DTOs
{
    public class TicketHistoryDto
    {
        public Guid Id { get; set; }
        public Guid QueueTicketId { get; set; }
        public TicketStatus OldStatus { get; set; }
        public TicketStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
    }
}
