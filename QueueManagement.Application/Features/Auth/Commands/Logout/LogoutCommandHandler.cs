using MediatR;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Unit.Value;
            }
            var success = await _authService.LogoutAsync(request.RefreshToken.ToString());

            if (!success)
            {
                
                throw new BadRequestException("Logout operation failed");
            }

            return Unit.Value;

            
        }
    }
}
