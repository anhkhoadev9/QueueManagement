using MediatR;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Commands.SubmitFeedback
{
    public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitFeedbackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.QueueTicketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            var feedback = new Feedback(request.TicketId, request.ServiceId, request.Rating, request.Comment);
            
            await _unitOfWork.FeedbackRepository.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
