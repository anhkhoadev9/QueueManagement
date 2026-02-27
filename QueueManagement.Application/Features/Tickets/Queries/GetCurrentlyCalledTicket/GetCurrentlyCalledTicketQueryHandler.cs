using MediatR;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetCurrentlyCalledTicket
{
    public class GetCurrentlyCalledTicketQueryHandler : IRequestHandler<GetCurrentlyCalledTicketQuery, TicketDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentlyCalledTicketQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto?> Handle(GetCurrentlyCalledTicketQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.QueueTicketRepository.GetCurrentlyCalledTicketAsync();

            if (ticket == null) return null;

            return new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerName = ticket.CustomerName,
                PhoneNumber = ticket.PhoneNumber,
                ServiceName = ticket.Service?.Name ?? "N/A",
                Status = ticket.Status,
                IssuedAt = ticket.IssuedAt,
                CalledAt = ticket.CalledAt
            };
        }
    }
}
