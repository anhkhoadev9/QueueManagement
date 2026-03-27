using MediatR;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.UpdateFeedBack
{
    public class UpdateFeedBackCommandHandler : IRequestHandler<UpdateFeedBackCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateFeedBackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(UpdateFeedBackCommand request, CancellationToken cancellationToken)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(request.Id);
            if (feedback == null)
                throw new NotFoundException("Not found feedback");
            feedback.EditFeedback(request.Rating ?? 0, request.Comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
