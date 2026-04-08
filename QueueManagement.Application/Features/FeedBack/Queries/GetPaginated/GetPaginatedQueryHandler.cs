using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetPaginated
{
    public class GetPaginatedQueryHandler : IRequestHandler<GetPaginatedQuery, PaginatedResult<FeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPaginatedQueryHandler(IUnitOfWork unitOfWork) {

            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<FeedbackDto>> Handle(GetPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.FeedbackRepository.GetFeedbackQueryWithDetails();

            var convertToDto = new PaginationRequestDto
            {
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                MaxPageSize = request.MaxPageSize,
                IncludeTicketDetails = request.IncludeTicketDetails,
            };

            var feedback = await _unitOfWork.FeedbackRepository.GetPaginatedAsync<FeedbackDto>(
               query,
               convertToDto,
               s => new FeedbackDto
               {
                   Id = s.Id,
                   Comment = s.Comment,
                   Rating = s.Rating,
                   SubmittedAt = s.SubmittedAt,
                   UserId = s.UserId ?? Guid.Empty,
                   UserName = s.User != null ? s.User.FullName : "Khách vãng lai",
                   ServiceName = s.Service != null ? s.Service.Name : (s.QueueTicket != null && s.QueueTicket.Service != null ? s.QueueTicket.Service.Name : string.Empty),
                   QueueTicketId = s.QueueTicketId,
                   TicketNumber = s.QueueTicket != null ? s.QueueTicket.TicketNumber : null,
                   CustomerName = s.QueueTicket != null ? s.QueueTicket.CustomerName : null
               },
               cancellationToken
            );

            return feedback;
        }
    }
}
