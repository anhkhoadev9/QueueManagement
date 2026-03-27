using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Linq.Expressions;


namespace QueueManagement.Application.Features.Service.Queries
{
    public class GetPaginatedResultServiceQueryHandler : IRequestHandler<GetPaginatedResultServiceQuery, PaginatedResult<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPaginatedResultServiceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<ServiceDto>> Handle(GetPaginatedResultServiceQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ServiceRepository.Query();
            var convertToDto = new PaginationRequestDto
            {
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                MaxPageSize = request.MaxPageSize,
                IncludeTicketDetails = request.IncludeTicketDetails,
            };

            var service = await _unitOfWork.ServiceRepository.GetPaginatedAsync<ServiceDto>(
                 query,
                convertToDto,
               s => new ServiceDto
               {
                   Id = s.Id,
                   Name = s.Name,
                   Description = s.Description,
                   EstimatedDurationMinus = s.EstimatedDurationMinus,
                   IsActive = s.IsActive,


               },
             cancellationToken
             );

            return service;
        }
    }
}
