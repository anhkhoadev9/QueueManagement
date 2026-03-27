using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetPaginatedTickets
{
    public class GetPaginatedTicketsQueryHandler : IRequestHandler<GetPaginatedTicketsQuery, PaginatedResult<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPaginatedTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<TicketDto>> Handle(GetPaginatedTicketsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.QueueTicketRepository.Query();
            
            var paginationDto = new PaginationRequestDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                MaxPageSize = request.MaxPageSize
            };


            var result = await _unitOfWork.QueueTicketRepository.GetPaginatedAsync<TicketDto>(
                query,
                paginationDto,
                t => new TicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    CustomerName = t.CustomerName,
                    PhoneNumber = t.PhoneNumber,
                    ServiceName = t.Service.Name,
                    Status = t.Status,
                    IssuedAt = t.IssuedAt,
                    CalledAt = t.CalledAt
                },
                cancellationToken);

            return result;
        }
    }
}
