using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetPaginateByQueueTicket
{
    public class GetPaginateFeedBackByQueueTicketQuery : PaginationRequestDto, IRequest<PaginatedResult<FeedbackDto>>
    {


        public Guid QueueTicketId { get; set; }



    }
}
