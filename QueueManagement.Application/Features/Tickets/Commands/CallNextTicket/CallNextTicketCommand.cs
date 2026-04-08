using MediatR;
using QueueManagement.Application.DTOs;

namespace QueueManagement.Application.Features.Tickets.Commands.CallNextTicket
{
    public class CallNextTicketCommand : IRequest <TicketDto>
    {
        public string StaffId { get; set; } = null!;
    }
}
