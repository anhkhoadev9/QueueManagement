using MediatR;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.UpdatedService
{
    public class UpdatedServiceCommandHandler : IRequestHandler<UpdatedServiceCommands, Unit>
    {
        private IUnitOfWork _unitOfWork;
        public UpdatedServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(UpdatedServiceCommands commands, CancellationToken cancellationToken)
        {

            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(commands.Id);
            if (service == null)
            {
                throw new NotFoundException("Service not found", commands.Id);
            }
            service.UpdateDetails(commands.Name, commands.Description, commands.EstimatedDurationMinus);
            
            _unitOfWork.ServiceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }


    }
}
