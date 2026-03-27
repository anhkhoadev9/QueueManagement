using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetFeedbackByServiceId
{
    public class GetFeedbackByServiceIdQueryHandler : IRequestHandler<GetFeedbackByServiceIdQuery, PaginatedResult<FeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFeedbackByServiceIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<FeedbackDto>> Handle(GetFeedbackByServiceIdQuery request, CancellationToken cancellationToken)
        {
            // Feedback usually links to QueueTicket, and QueueTicket links to Service
            var feedback = _unitOfWork.FeedbackRepository.Query().Where(s => s.ServiceId == request.ServiceId);


            var pagination = new Domain.DTOs.PaginationRequestDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                MaxPageSize = request.MaxPageSize

            };
            return await _unitOfWork.FeedbackRepository.GetPaginatedAsync(
                feedback,
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
