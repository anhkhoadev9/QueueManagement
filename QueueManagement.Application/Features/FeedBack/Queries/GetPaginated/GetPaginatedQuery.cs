using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Queries.GetPaginated
{
    public class GetPaginatedQuery: PaginationRequestDto, IRequest<PaginatedResult< FeedbackDto>>
    {
    }
}
