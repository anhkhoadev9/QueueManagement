using MediatR;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.DeletedService
{
    public class DeletedServiceCommandHandler : IRequestHandler<DeletedServiceCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeletedServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeletedServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.Id);
            if (service == null)
            {
                throw new NotFoundException("Service not found", request.Id);
            }

            _unitOfWork.ServiceRepository.Delete(service);
            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
