using MediatR;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.ExternalLogin
{
    public class GoogleLoginCommandHandler
    : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;
        

        public GoogleLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDto> Handle(GoogleLoginCommand request,CancellationToken cancellationToken)
        {
            return await _authService.GoogleLoginAsync(request.code, cancellationToken);
        }
    }
}
