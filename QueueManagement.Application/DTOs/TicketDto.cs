using QueueManagement.Domain.Enum;
using System;

namespace QueueManagement.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public TicketStatus Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? CalledAt { get; set; }
    }
}
