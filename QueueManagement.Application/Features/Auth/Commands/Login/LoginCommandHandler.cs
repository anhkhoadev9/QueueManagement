using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {

        private readonly IAuthService _identityService;
       
        public LoginCommandHandler(IAuthService identityService)
        {
            _identityService = identityService;
           
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request , CancellationToken cancellationToken)
        {
            var result = await _identityService.LoginAsync(request.LoginInfo, request.Password);
            return result;
        }

    }
}


