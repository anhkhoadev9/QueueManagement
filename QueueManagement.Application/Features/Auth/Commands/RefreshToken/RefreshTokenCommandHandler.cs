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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
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

        public async Task<RefreshTokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (token == null || token.IsRevoked || token.IsExpired())
                throw new UnauthorizedAccessException();
            
            //  lấy domain user
            var user = await _userRepository
                .GetByIdAsync(token.UserId, cancellationToken);
            if (user == null) throw new NotFoundException("Not found user");
            //  lấy identityId từ user
            var identityId = user.IdentityUserId;
            // lấy role
            var roles = await _roleService.GetUserRoles(identityId);
            // revoke token cũ
            token.Revoke();

            // tạo token mới
            var generate = await _jwtTokenGenerator.GenerateTokenAsync(
                identityId,
                user.Email,
                roles
            );
            // tạo refresh token mới
            var newRefresh = QueueManagement.Domain.Entities.RefreshToken.Create(user.Id, generate.RefreshToken);

            await _refreshTokenRepository.AddAsync(newRefresh);
            await _unitOfWork.SaveChangesAsync();
            var result = new RefreshTokenDto
            {
                RefreshToken = generate.RefreshToken,

            };
            return result;
        }

    }
}
