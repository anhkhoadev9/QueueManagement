using MediatR;
using QueueManagement.Domain.Enum;
using System;

namespace QueueManagement.Application.Features.Tickets.Commands.UpdateTicketStatus
{
    public class UpdateTicketStatusCommand : IRequest<bool>
    {
        public Guid TicketId { get; set; }
        public TicketStatus NewStatus { get; set; }
        public string StaffId { get; set; } = null!;
    }
}
