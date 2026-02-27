using MediatR;
using QueueManagement.Application.DTOs;
using System.Collections.Generic;

namespace QueueManagement.Application.Features.Tickets.Queries.GetWaitingTickets
{
    public class GetWaitingTicketsQuery : IRequest<List<TicketDto>>
    {
    }
}
