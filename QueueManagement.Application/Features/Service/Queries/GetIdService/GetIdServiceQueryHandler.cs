using MediatR;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Queries.GetIdService
{
    public class GetIdServiceQueryHandler : IRequestHandler<GetIdServiceQuery, Domain.Entities.Service>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetIdServiceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Domain.Entities.Service> Handle(GetIdServiceQuery request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.Id);
            if (service == null)
            {
                throw new NotFoundException("Not Found Service");
            }
            return service;
        }
    }
}
