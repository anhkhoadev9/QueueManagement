using Microsoft.AspNetCore.Identity;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<bool> EmailExistsAsync(string email);
        
        Task<bool> UsernameExistsAsync(string username);

        Task<Guid> CreateUserAsync(
            string username,
            string email,
            string password,
            string phoneNumber,
            Guid domainUserId);

        Task<AuthUserDto> CheckPasswordAsync(string login, string password);
        Task<AuthResponseDto> LoginAsync(string loginInfo, string password, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> RefreshToken( string refreshtoken, CancellationToken cancellationToken);
        Task<bool> LogoutAsync(string? refreshToken = null);
        Task<bool> ValidateCredentialsAsync(string loginInfo, string password);
        Task<string> ForgotPassword(string email,CancellationToken cancellationToken=default);
        Task<bool> ChangePasswordAsync( string oldPassword,string newPassword,CancellationToken cancellation=default);

        Task<AuthResponseDto> ExchangeGoogleCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<GoogleUserDto> GetGoogleUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
        Task<(UserDto domainUser, Guid identityId)> FindOrCreateGoogleUserAsync(GoogleUserDto googleUser, CancellationToken cancellationToken = default);
        public   Task<AuthResponseDto> GoogleLoginAsync(string code, CancellationToken cancellationToken = default);
        }
}
