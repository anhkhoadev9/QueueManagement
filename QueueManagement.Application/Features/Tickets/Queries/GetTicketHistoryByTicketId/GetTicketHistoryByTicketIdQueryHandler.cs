using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetTicketHistoryByTicketId
{
    public class GetTicketHistoryByTicketIdQueryHandler : IRequestHandler<GetTicketHistoryByTicketIdQuery, PaginatedResult<TicketHistoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTicketHistoryByTicketIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<TicketHistoryDto>> Handle(GetTicketHistoryByTicketIdQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.TicketHistoryRepository.Query().Where(h => h.QueueTicketId == request.TicketId);
            var paginationDto = new PaginationRequestDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                MaxPageSize = request.MaxPageSize
            };
            return await _unitOfWork.TicketHistoryRepository.GetPaginatedAsync(query, paginationDto, h => new TicketHistoryDto
            {
                Id = h.Id,
                QueueTicketId = h.QueueTicketId,
                OldStatus = h.OldStatus,
                NewStatus = h.NewStatus,
                ChangedAt = h.ChangedAt,
                ChangedBy = h.ChangedBy
            });




        }
    }
}
