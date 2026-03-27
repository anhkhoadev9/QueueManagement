using MediatR;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.ChangeStatusService
{
    public class LockServiceCommandHandler : IRequestHandler<LockServiceCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LockServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(LockServiceCommand request, CancellationToken cancellationToken)
        {

            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.Id);
            if (service == null)
            {
                throw new NotFoundException("Service not found", request.Id);
            }

            service.LockService();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
