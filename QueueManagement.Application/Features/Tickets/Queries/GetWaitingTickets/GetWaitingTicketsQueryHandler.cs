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
            // Trong thực tế, có thể viết thêm method trong Repository như GetWaitingTicketsAsync() để optimize query DB
            var tickets = await _unitOfWork.QueueTicketRepository.GetAsync(t => t.Status == TicketStatus.Waiting);

            var ticketDtos = tickets.OrderBy(t => t.IssuedAt).Select(ticket => new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerName = ticket.CustomerName,
                PhoneNumber = ticket.PhoneNumber,
                ServiceName = "TBD", // Trong thực tế lấy qua service include
                Status = ticket.Status,
                IssuedAt = ticket.IssuedAt,
                CalledAt = ticket.CalledAt
            }).ToList();

            return ticketDtos;
        }
    }
}
