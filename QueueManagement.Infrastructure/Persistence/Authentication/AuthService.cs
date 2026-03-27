using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Exceptions;
using QueueManagement.Application.Features.Auth.Commands.Login;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Domain.Validators;
using QueueManagement.Infrastructure.Identity;
using QueueManagement.Infrastructure.Persistence.Context;
using QueueManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly QueueManagementDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWrok;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;// Thêm SignInManager
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(
            UserManager<ApplicationUser> userManager, QueueManagementDbContext context, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWrok, IPasswordGenerator passwordGenerator, IEmailLogRepository emailLogRepository,IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator, IRoleService roleService, IUnitOfWork unitOfWork) // Thêm context
        {
            _userManager = userManager;
            _context = context;
            _contextAccessor = contextAccessor;
            _unitOfWrok = unitOfWrok;
            _passwordGenerator = passwordGenerator;
            _emailLogRepository = emailLogRepository;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }
        #region Register
        public async Task<Guid> CreateUserAsync(string username, string email, string password,
            string phoneNumber, Guid domainUserId)
        {
            var user = new ApplicationUser()
            {
                UserName = username,
                Email = email,
                PhoneNumber = phoneNumber,
                UserId = domainUserId
            };

            // UserManager tự động dùng transaction của context
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Identity", result.Errors.Select(e => e.Description).ToArray() }
            });
            }

            await _userManager.AddToRoleAsync(user, "User");
            return user.Id;
        }
        public async Task<bool> EmailExistsAsync(string email)
            => await _userManager.FindByEmailAsync(email) != null;


        // trả về true nếu là có email tồn tại trước đó
        public async Task<bool> UsernameExistsAsync(string username)
            => await _userManager.FindByNameAsync(username) != null;

        public async Task<AuthUserDto> CheckPasswordAsync(string login, string password)
        {

            var user = await _userManager.Users
         .FirstOrDefaultAsync(u => u.Email == login || u.UserName == login);
            if (user == null)
                throw new UnauthorizedException("Invalid credentials");

            var check = await _userManager.CheckPasswordAsync(user, password);

            if (!check)
                throw new UnauthorizedException("Invalid credentials");
            var reuslt = new AuthUserDto
            {
                UserId = user.UserId,
                Email = user.Email ?? "anonymous",
                IdentityId = user.Id

            };
            return reuslt;
        }
        #endregion
        #region Logout
        public async Task<bool> LogoutAsync(string? refreshToken = null)
        {
            // 1. Invalidate session (security stamp) - force logout tất cả thiết bị nếu cần
            var userId = _contextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
            }

            // 2. Revoke refresh token cụ thể (nếu client gửi lên)
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token != null)
                {

                    token.Revoke();
                    await _unitOfWrok.SaveChangesAsync();  // Nếu dùng DbContext trực tiếp
                }
                // Nếu không tìm thấy token → vẫn coi như OK (có thể đã revoke trước)
            }

            // 3. Clear local session (chỉ nếu dùng cookie-based auth)
            if (_contextAccessor.HttpContext != null)
            {
                await _contextAccessor.HttpContext.SignOutAsync();
            }



            return true;
        }

#endregion
        #region ForgotPassword
        public async Task<string> ForgotPassword(string email, CancellationToken cancellationToken = default)
        {
            var subject = "Yêu cầu khôi phục mật khẩu";
            var input = email.Trim();
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == input);
            if (user == null)
                throw new NotFoundException("Email");

            //Tạo mật khẩu mới
            var newPassword = _passwordGenerator.Generate();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset password bằng token
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to reset password: {errors}");
            }
            var body = $@"<h3>Khôi phục mật khẩu</h3><p>Mật khẩu mới của bạn: <strong>{newPassword}</strong></p><p>Vui lòng đổi mật khẩu sau khi đăng nhập.</p>";
            await _emailLogRepository.SendAsync(input, subject, body);
            await _userManager.UpdateAsync(user); ;
            return newPassword;
        }
#endregion
        #region ChangePassword
        public async Task<bool> ChangePasswordAsync(
     string oldPassword,
     string newPassword,
     CancellationToken cancellationToken = default)
        {
            // 1. Validate new password
            var (isValid, errors) = PasswordValidator.Validate(newPassword);
            if (!isValid)
            {
                var errorDict = new Dictionary<string, string[]>
        {
            { "NewPassword", errors.ToArray() }
        };
                throw new ValidationException(errorDict);
            }

            // 2. Kiểm tra HttpContext
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }

            // 3. Lấy user hiện tại từ HttpContext (không cần tìm lại bằng email)
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser == null)
            {
                throw new UnauthorizedException("User not found or not authenticated");
            }

            // 4. Kiểm tra user có password không
            if (!await _userManager.HasPasswordAsync(currentUser))
            {
                throw new BadRequestException("User does not have a password set. Please use external login.");
            }

            // 5. Đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            // 6. Kiểm tra kết quả
            if (!result.Succeeded)
            {
                var errorsMsg = string.Join(", ", result.Errors.Select(e => e.Description));

                // Phân biệt lỗi để trả về đúng status code
                if (errorsMsg.Contains("Incorrect password") ||
                    errorsMsg.Contains("incorrect password") ||
                    errorsMsg.Contains("Invalid password"))
                {
                    throw new UnauthorizedException("Old password is incorrect");
                }

                throw new BadRequestException($"Change password failed: {errorsMsg}");
            }

            return true;
        }
#endregion


        public async Task<bool> ValidateCredentialsAsync(string loginInfo, string password)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == loginInfo || u.UserName == loginInfo);

            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }
        #region Login
        public async Task<AuthResponseDto> LoginAsync(string loginInfo, string password, CancellationToken cancellationToken = default)
        {
            // 1. Tìm user
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == loginInfo || u.UserName == loginInfo, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            // 2. Kiểm tra lockout trước
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                throw new UnauthorizedException($"Account is locked. Please try again after {lockoutEnd?.Subtract(DateTimeOffset.UtcNow).Minutes} minutes.");
            }

            // 3. Check password với lockoutOnFailure = true
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                throw new UnauthorizedException($"Too many failed attempts. Account locked for {lockoutEnd?.Subtract(DateTimeOffset.UtcNow).Minutes} minutes.");
            }

            if (!result.Succeeded)
            {
                // Tăng số lần đếm sai đã được tự động bởi CheckPasswordSignInAsync
                var remainingAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts -
                                       await _userManager.GetAccessFailedCountAsync(user);
                throw new UnauthorizedException($"Invalid credentials. You have {remainingAttempts} attempts remaining.");
            }

            // 4. Reset số lần đếm sai khi đăng nhập thành công
            await _userManager.ResetAccessFailedCountAsync(user);

            // 5. Lấy thông tin từ domain
            var domainUser = await _userRepository.GetInforUser(loginInfo, cancellationToken);
            var roles = await _roleService.GetUserRoles(user.Id);

            // 6. Generate tokens
            var tokens = await _jwtTokenGenerator.GenerateTokenAsync(user.UserId, user.Email, roles);

            // 7. Save refresh token
            var refreshToken = QueueManagement.Domain.Entities.RefreshToken.Create(domainUser.Id, tokens.RefreshToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Return response
            return new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.ExpiresAt,
                ExpiresIn = tokens.ExpiresIn,
                TokenType = tokens.TokenType
            };
        }
        #endregion
    }
}
