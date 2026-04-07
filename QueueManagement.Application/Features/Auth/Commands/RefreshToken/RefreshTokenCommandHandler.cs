using MediatR;
using Microsoft.AspNetCore.Http;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RefreshTokenCommandHandler(IAuthService authService, IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IHttpContextAccessor httpContextAccessor, IRoleService roleService, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshToken(request.RefreshToken, cancellationToken);
            return result;
        }

    }
}
