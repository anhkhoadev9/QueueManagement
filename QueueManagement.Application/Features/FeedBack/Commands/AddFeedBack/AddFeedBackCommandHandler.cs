using MediatR;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.AddFeedBack
{
    public class AddFeedBackCommandHandler : IRequestHandler<AddFeedBackCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddFeedBackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(AddFeedBackCommand request, CancellationToken cancellationToken)
        {
            var feedback = FeedBackMapping.ToEntity(request);
            await _unitOfWork.FeedbackRepository.AddAsync(feedback, cancellationToken);
            return Unit.Value;
        }
    }
}
