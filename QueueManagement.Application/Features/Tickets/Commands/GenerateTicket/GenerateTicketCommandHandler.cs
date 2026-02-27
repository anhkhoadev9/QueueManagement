using MediatR;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Commands.GenerateTicket
{
    public class GenerateTicketCommandHandler : IRequestHandler<GenerateTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueHubService _hubService;

        public GenerateTicketCommandHandler(IUnitOfWork unitOfWork, IQueueHubService hubService)
        {
            _unitOfWork = unitOfWork;
            _hubService = hubService;
        }

        public async Task<TicketDto> Handle(GenerateTicketCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            // Logic generate số tự động (Ví dụ: đếm số ticket trong ngày + 1)
            int todayCount = await _unitOfWork.QueueTicketRepository.GetTodayTicketCountAsync();
            int nextNumber = todayCount + 1;
            string ticketNumber = nextNumber.ToString("D4"); // Ví dụ: 0001, 0002

            var ticket = new QueueTicket(ticketNumber, request.CustomerName, request.PhoneNumber, request.ServiceId);

            await _unitOfWork.QueueTicketRepository.AddAsync(ticket);

            // Log history
            var history = new TicketHistory(ticket.Id, ticket.Status, ticket.Status, "Kiosk");
            await _unitOfWork.TicketHistoryRepository.AddAsync(history);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerName = ticket.CustomerName,
                PhoneNumber = ticket.PhoneNumber,
                ServiceName = service.Name,
                Status = ticket.Status,
                IssuedAt = ticket.IssuedAt
            };

            await _hubService.NotifyTicketCreated(result);

            return result;
        }
    }
}
