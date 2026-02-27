using MediatR;
using QueueManagement.Application.DTOs;
using System;

namespace QueueManagement.Application.Features.Tickets.Commands.GenerateTicket
{
    public class GenerateTicketCommand : IRequest<TicketDto>
    {
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public Guid ServiceId { get; set; }
    }
}
