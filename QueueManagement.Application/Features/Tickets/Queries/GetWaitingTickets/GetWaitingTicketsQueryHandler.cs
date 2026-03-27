using MediatR;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetWaitingTickets
{
    public class GetWaitingTicketsQueryHandler : IRequestHandler<GetWaitingTicketsQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWaitingTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetWaitingTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.QueueTicketRepository.GetWaitingTicketsWithServiceAsync(cancellationToken);
 
            var ticketDtos = tickets.Select(ticket => new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerName = ticket.CustomerName,
                PhoneNumber = ticket.PhoneNumber,
                ServiceName = ticket.Service?.Name ?? "N/A",
                Status = ticket.Status,
                IssuedAt = ticket.IssuedAt,
                CalledAt = ticket.CalledAt
            }).ToList();
 
            return ticketDtos;
        }
    }
}
