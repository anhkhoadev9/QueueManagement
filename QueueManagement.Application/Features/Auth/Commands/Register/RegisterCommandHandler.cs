using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IAuthService _identityService;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, IAuthService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;

        }
        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var errEmail = new Dictionary<string, string[]> { { "Email", new[] { "Email already exists" } } };
            var errUsername = new Dictionary<string, string[]> { { "UserName", new[] { "Username already exists" } } };
            // 1 Check email
            var existEmail = await _identityService.EmailExistsAsync(request.Email);
            if (existEmail) throw new ValidationException(errEmail);
            // 2 Check username
            var existUsername = await _identityService.UsernameExistsAsync(request.UserName);
            if (existUsername) throw new ValidationException(errUsername);
            
            if (request.Password != request.ConfirmPassword)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                { "ConfirmPassword", new[] { "Mật khẩu xác nhận không khớp" } }
            });
            }
            //Đây là bước bắt đầu transaction(**)
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //3 Create User( Domain/Entity)
                var user = request.ToEntity();
                var result = await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);//Lưu vào database
                //4 Create IdentityUser
                await _identityService.CreateUserAsync(request.UserName, request.Email, request.Password, request.PhoneNumber, result.Id);
                //5 Transaction Lần cuối==> Thành công thì commit còn fail thì sẽ rollback
                await _unitOfWork.CommitTransactionAsync();
                return Unit.Value;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }

    }
}

