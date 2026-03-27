using MediatR;
using QueueManagement.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IAuthService _authService;
        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _authService.ChangePasswordAsync(request.OldPassword, request.NewPassword, cancellationToken);
            return Unit.Value;
        }
    }
}
