using MediatR;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.DeleteFeedBack
{
    public class DeleteFeedBackCommandHandler : IRequestHandler<DeleteFeedBackCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteFeedBackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteFeedBackCommand request, CancellationToken cancellationToken)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(request.Id);
            if (feedback == null)
                throw new NotFoundException("Not found feedback");
            feedback.SoftDeleteFeedback();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;

        }
    }
}
