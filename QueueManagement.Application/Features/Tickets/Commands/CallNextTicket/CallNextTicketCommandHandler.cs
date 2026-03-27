using MediatR;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Commands.CallNextTicket
{
    public class CallNextTicketCommandHandler : IRequestHandler<CallNextTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CallNextTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(CallNextTicketCommand request, CancellationToken cancellationToken)
        {
            // Tìm vé đang đợi lâu nhất (IssuedAt cũ nhất)
            var waitingTickets = await _unitOfWork.QueueTicketRepository.GetWaitingTicketsWithServiceAsync(cancellationToken);
            var nextTicket = waitingTickets.FirstOrDefault();

            if (nextTicket == null)
            {
                throw new Exception("No tickets in waiting queue.");
            }

            var oldStatus = nextTicket.Status;
            nextTicket.MarkAsCalled();

            _unitOfWork.QueueTicketRepository.Update(nextTicket);

            // Log history
            var history = new QueueManagement.Domain.Entities.TicketHistory(nextTicket.Id, oldStatus, nextTicket.Status, request.StaffId);
            await _unitOfWork.TicketHistoryRepository.AddAsync(history);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // TODO: Bắn event SignalR qua IQueueHubService (nếu đã có service này)

            return new TicketDto
            {
                Id = nextTicket.Id,
                TicketNumber = nextTicket.TicketNumber,
                CustomerName = nextTicket.CustomerName,
                PhoneNumber = nextTicket.PhoneNumber,
                ServiceName = nextTicket.Service?.Name ?? "N/A",
                Status = nextTicket.Status,
                IssuedAt = nextTicket.IssuedAt,
                CalledAt = nextTicket.CalledAt
            };
        }
    }
}
