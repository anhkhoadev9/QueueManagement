using MediatR;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;
using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Users.Queries.GetPaginatedUsers
{
    public class GetPaginatedUsersQuery : IRequest<PaginatedResult<UserDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 50;
    }

    public class GetPaginatedUsersQueryHandler : IRequestHandler<GetPaginatedUsersQuery, PaginatedResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPaginatedUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetPaginatedUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserRepository.Query();
            
            var paginationDto = new PaginationRequestDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                MaxPageSize = request.MaxPageSize
            };

            var result = await _unitOfWork.UserRepository.GetPaginatedAsync<UserDto>(
                query,
                paginationDto,
                u => new UserDto
                {
                    Id = u.Id,
                    Code = u.Code,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    BirthDay = u.BirthDay ?? default,
                    StatusUser = u.StatusUser,
                    ProviderName = u.ProviderName
                },
                cancellationToken);

            return result;
        }
    }
}
