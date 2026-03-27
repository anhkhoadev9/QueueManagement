using MediatR;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.CreatedService
{
    public class CreatedServiceCommandHandler : IRequestHandler<CreatedServiceCommands, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreatedServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CreatedServiceCommands request, CancellationToken cancellationToken)
        {
           
            await _unitOfWork.ServiceRepository.AddAsync(ServiceMapping.ToEntity(request), cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); 

            return Unit.Value;
        }
    }
}
