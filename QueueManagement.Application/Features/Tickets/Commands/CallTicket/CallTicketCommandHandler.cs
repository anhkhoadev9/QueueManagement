using MediatR;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Commands.CallTicket
{
    public class CallTicketCommandHandler : IRequestHandler<CallTicketCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CallTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CallTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.QueueTicketRepository.GetByIdAsync(request.TicketId);
            
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            var oldStatus = ticket.Status;
            
            ticket.MarkAsCalled();


            _unitOfWork.QueueTicketRepository.Update(ticket);

            // Log history
            var history = new TicketHistory(ticket.Id, oldStatus, ticket.Status, request.StaffId);
            await _unitOfWork.TicketHistoryRepository.AddAsync(history);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // TODO: Bắn event (SignalR) cho màn hình TV biết ở tầng Infrastructure/Presentation
            
            return true;
        }
    }
}
