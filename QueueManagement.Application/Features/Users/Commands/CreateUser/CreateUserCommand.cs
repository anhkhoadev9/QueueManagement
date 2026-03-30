using MediatR;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _identityService;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IAuthService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var errEmail = new Dictionary<string, string[]> { { "Email", new[] { "Email already exists" } } };
            var errUsername = new Dictionary<string, string[]> { { "UserName", new[] { "Username already exists" } } };
            
            var existEmail = await _identityService.EmailExistsAsync(request.Email);
            if (existEmail) throw new ValidationException(errEmail);
            
            var existUsername = await _identityService.UsernameExistsAsync(request.UserName);
            if (existUsername) throw new ValidationException(errUsername);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User(request.FullName, request.Email, request.PhoneNumber, request.BirthDay,null, QueueManagement.Domain.Enum.StatusUser.Active);
                
                await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                await _identityService.CreateUserAsync(request.UserName, request.Email, request.Password, request.PhoneNumber, user.Id);
                
                await _unitOfWork.CommitTransactionAsync();
                
                return user.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
