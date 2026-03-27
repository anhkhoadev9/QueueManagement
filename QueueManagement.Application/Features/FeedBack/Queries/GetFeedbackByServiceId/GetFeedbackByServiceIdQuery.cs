using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Interfaces;
using System.Collections.Generic;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetFeedbackByServiceId
{
    public class GetFeedbackByServiceIdQuery : PaginationRequestDto, IRequest<PaginatedResult<FeedbackDto>>
    {
        public Guid ServiceId { get; set; }
    }
}
