using MediatR;
using QueueManagement.Application.DTOs;

namespace QueueManagement.Application.Features.Tickets.Queries.GetCurrentlyCalledTicket
{
    public class GetCurrentlyCalledTicketQuery : IRequest<TicketDto?>
    {
    }
}
