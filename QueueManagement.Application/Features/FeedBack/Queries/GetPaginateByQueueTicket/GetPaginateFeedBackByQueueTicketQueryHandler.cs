using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetPaginateByQueueTicket
{
    internal class GetPaginateFeedBackByQueueTicketQueryHandler : IRequestHandler<GetPaginateFeedBackByQueueTicketQuery, PaginatedResult<FeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPaginateFeedBackByQueueTicketQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<FeedbackDto>> Handle(GetPaginateFeedBackByQueueTicketQuery request, CancellationToken cancellationToken)
        {


            var query = _unitOfWork.FeedbackRepository.Query()
                .Where(f => f.QueueTicketId == request.QueueTicketId);

            var pagination = new PaginationRequestDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                MaxPageSize = request.MaxPageSize

            };

            return await _unitOfWork.FeedbackRepository.GetPaginatedAsync(
                query,
                pagination,
                f => new FeedbackDto
                {
                    Id = f.Id,
                    QueueTicketId = f.QueueTicketId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    SubmittedAt = f.SubmittedAt,

                    TicketNumber = f.QueueTicket.TicketNumber,
                    CustomerName = f.QueueTicket.CustomerName,
                    ServiceName = f.QueueTicket.Service.Name
                },
                cancellationToken);
        }
    }
}