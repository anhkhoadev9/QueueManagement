using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using System.Collections.Generic;

namespace QueueManagement.Application.Features.Tickets.Queries.GetTicketHistoryByTicketId
{
    public class GetTicketHistoryByTicketIdQuery :PaginationRequestDto, IRequest<PaginatedResult<TicketHistoryDto>>
    {
        public Guid TicketId { get; set; }
    }
}
