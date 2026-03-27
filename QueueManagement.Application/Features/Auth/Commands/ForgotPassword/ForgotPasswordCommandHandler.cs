using MediatR;
using Microsoft.AspNetCore.Identity;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IAuthService _authService;
        

        private readonly IEmailLogRepository _emailLogRepository;
        public ForgotPasswordCommandHandler(IAuthService authService, IEmailLogRepository emailLogRepository )
        {
            _authService = authService;
            _emailLogRepository = emailLogRepository;

        }
        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
           
            var newPassword = await _authService.ForgotPassword(request.Email, cancellationToken);
            if (string.IsNullOrEmpty(newPassword)) throw new BadRequestException("Generate password fail");
            return Unit.Value;
        }


    }
}
