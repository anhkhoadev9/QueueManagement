using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Entities.Abstractions;



namespace QueueManagement.Application.Features.Service.Queries
{
    public class GetPaginatedResultServiceQuery : PaginationRequestDto, IRequest<PaginatedResult<ServiceDto>>
    {


    }
}
