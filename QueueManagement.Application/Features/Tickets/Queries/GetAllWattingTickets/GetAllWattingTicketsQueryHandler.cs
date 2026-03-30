using MediatR;
using QueueManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetAllWattingTickets
{
    public class GetAllWattingTicketsQueryHandler : IRequestHandler<GetAllWattingTicketsQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllWattingTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<List<TicketDto>> Handle(GetAllWattingTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.QueueTicketRepository.GetAllWaitingTicketsWithServiceAsync(cancellationToken);

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
