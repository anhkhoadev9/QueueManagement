using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;

namespace QueueManagement.Application.Features.Tickets.Queries.GetPaginatedTickets
{
    public class GetPaginatedTicketsQuery : IRequest<PaginatedResult<TicketDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 50;
    }
}
