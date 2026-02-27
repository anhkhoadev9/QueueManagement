using MediatR;
using System;

namespace QueueManagement.Application.Features.Tickets.Commands.CallTicket
{
    public class CallTicketCommand : IRequest<bool>
    {
        public Guid TicketId { get; set; }
        public string StaffId { get; set; } = null!;
    }
}
