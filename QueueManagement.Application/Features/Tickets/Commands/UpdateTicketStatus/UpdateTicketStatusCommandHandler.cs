using MediatR;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Commands.UpdateTicketStatus
{
    public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueHubService _hubService;

        public UpdateTicketStatusCommandHandler(IUnitOfWork unitOfWork, IQueueHubService hubService)
        {
            _unitOfWork = unitOfWork;
            _hubService = hubService;
        }

        public async Task<bool> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.QueueTicketRepository.GetTicketWithDetailsAsync(request.TicketId, cancellationToken);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            var oldStatus = ticket.Status;

            switch (request.NewStatus)
            {
                case TicketStatus.InProgress:
                    ticket.MarkAsInProgress();
                    break;
                case TicketStatus.Completed:
                    ticket.MarkAsCompleted();
                    break;
                case TicketStatus.Cancelled:
                    ticket.MarkAsCancelled();
                    break;
                case TicketStatus.Skipped:
                    ticket.MarkAsSkipped();
                    break;
                default:
                    throw new Exception("Invalid status transition.");
            }

            _unitOfWork.QueueTicketRepository.Update(ticket);

            var history = new TicketHistory(ticket.Id, oldStatus, ticket.Status, request.StaffId);
            await _unitOfWork.TicketHistoryRepository.AddAsync(history);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify via SignalR
            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerName = ticket.CustomerName,
                PhoneNumber = ticket.PhoneNumber,
                ServiceName = ticket.Service?.Name ?? string.Empty,
                ServiceId = ticket.ServiceId,
                Status = ticket.Status,
                CalledAt = ticket.CalledAt
            };

            if (ticket.Status == TicketStatus.Completed)
            {
                await _hubService.NotifyTicketCompleted(ticketDto);
            }
            else
            {
                await _hubService.NotifyTicketCalled(ticketDto);
            }

            await _hubService.NotifyQueueUpdated();

            return true;
        }
    }
}
