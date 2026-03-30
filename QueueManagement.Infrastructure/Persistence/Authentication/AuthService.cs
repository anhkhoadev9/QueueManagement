using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Exceptions;
using QueueManagement.Application.Features.Auth.Commands.Login;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Domain.Validators;
using QueueManagement.Infrastructure.Identity;
using QueueManagement.Infrastructure.Persistence.Context;
using QueueManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly QueueManagementDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;// Thêm SignInManager
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthService(
            UserManager<ApplicationUser> userManager, QueueManagementDbContext context, IHttpContextAccessor contextAccessor, IPasswordGenerator passwordGenerator, IEmailLogRepository emailLogRepository, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator, IRoleService roleService, IUnitOfWork unitOfWork, IConfiguration configuration, IHttpClientFactory httpClientFactory) // Thêm context
        {
            _userManager = userManager;
            _context = context;
            _contextAccessor = contextAccessor;

            _passwordGenerator = passwordGenerator;
            _emailLogRepository = emailLogRepository;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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
                    await _unitOfWork.SaveChangesAsync();  // Nếu dùng DbContext trực tiếp
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
            Console.WriteLine($"Password raw: {newPassword}");
            // Reset password bằng token
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            Console.WriteLine($"Reset success: {result.Succeeded}");
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to reset password: {errors}");
            }
            var body = $@"<h3>Khôi phục mật khẩu</h3><p>Mật khẩu mới của bạn: <strong>{newPassword}</strong></p><p>Vui lòng đổi mật khẩu sau khi đăng nhập.</p>";
            await _emailLogRepository.SendAsync(input, subject, body);
            /*await _userManager.UpdateAsync(user); */
            ;
            return newPassword;
        }
        #endregion
        #region ChangePassword
        //public async Task<bool> ChangePasswordAsync(string oldPassword,string newPassword,CancellationToken cancellationToken = default)
        //{
        //    // 1. Validate new password
        //    var (isValid, errors) = PasswordValidator.Validate(newPassword);
        //    if (!isValid)
        //    {
        //        var errorDict = new Dictionary<string, string[]>
        //{
        //    { "NewPassword", errors.ToArray() }
        //};
        //        throw new ValidationException(errorDict);
        //    }

        //    // 2. Kiểm tra HttpContext
        //    if (_httpContextAccessor.HttpContext == null)
        //    {
        //        throw new InvalidOperationException("HttpContext is not available");
        //    }

        //    // 3. Lấy user hiện tại từ HttpContext (không cần tìm lại bằng email)
        //    var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        //    if (currentUser == null)
        //    {
        //        throw new UnauthorizedException("User not found or not authenticated");
        //    }

        //    // 4. Kiểm tra user có password không
        //    if (!await _userManager.HasPasswordAsync(currentUser))
        //    {
        //        throw new BadRequestException("User does not have a password set. Please use external login.");
        //    }

        //    // 5. Đổi mật khẩu
        //    var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

        //    // 6. Kiểm tra kết quả
        //    if (!result.Succeeded)
        //    {
        //        var errorsMsg = string.Join(", ", result.Errors.Select(e => e.Description));

        //        // Phân biệt lỗi để trả về đúng status code
        //        if (errorsMsg.Contains("Incorrect password") ||
        //            errorsMsg.Contains("incorrect password") ||
        //            errorsMsg.Contains("Invalid password"))
        //        {
        //            throw new UnauthorizedException("Old password is incorrect");
        //        }

        //        throw new BadRequestException($"Change password failed: {errorsMsg}");
        //    }

        //    return true;
        //}
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

            var userPrincipal = _httpContextAccessor.HttpContext.User;

            // 3. Lấy UserId từ claims (thử nhiều claim types khác nhau)
            var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = userPrincipal.FindFirst("sub")?.Value;
            }

            // 4. Lấy email từ claims (phòng trường hợp)
            var email = userPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                email = userPrincipal.FindFirst("email")?.Value;
            }

            // Log để debug


            // 5. Tìm user bằng UserId
            ApplicationUser currentUser = null;

            if (!string.IsNullOrEmpty(userId))
            {
                currentUser = await _userManager.FindByIdAsync(userId);

            }

            // 6. Nếu không tìm thấy bằng Id, thử tìm bằng email
            if (currentUser == null && !string.IsNullOrEmpty(email))
            {
                currentUser = await _userManager.FindByEmailAsync(email);

            }

            // 7. Nếu vẫn không tìm thấy, thử tìm bằng username (nếu email là username)
            if (currentUser == null && !string.IsNullOrEmpty(email))
            {
                currentUser = await _userManager.FindByNameAsync(email);

            }

            if (currentUser == null)
            {

                throw new UnauthorizedException("User not found or not authenticated");
            }



            // 8. Kiểm tra user có password không
            if (!await _userManager.HasPasswordAsync(currentUser))
            {
                throw new BadRequestException("User does not have a password set. Please use external login.");
            }

            // 9. Đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            // 10. Kiểm tra kết quả
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
            var refreshToken = RefreshToken.Create(domainUser.Id, tokens.RefreshToken);
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
        public async Task<AuthResponseDto> ExchangeGoogleCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            // 1. Lấy cấu hình từ appsettings
            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];
            var redirectUri = _configuration["Google:RedirectUri"];

            // 2. Tạo HttpClient
            var httpClient = _httpClientFactory.CreateClient();

            // 3. Tạo form data (x-www-form-urlencoded)
            var tokenRequest = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            // 4. Gửi POST request đến Google
            var response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest),
                cancellationToken
            );

            // 5. Đọc response
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            // 6. Kiểm tra lỗi
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google token exchange failed: {content}");
            }

            // 7. Parse JSON response
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // 8. Trả về AuthResponseDto
            return new AuthResponseDto
            {
                AccessToken = tokenResponse.access_token,
                RefreshToken = tokenResponse.refresh_token,
                ExpiresIn = tokenResponse.expires_in,
                TokenType = tokenResponse.token_type
            };
        }

        public async Task<GoogleUserDto> GetGoogleUserInfoAsync(
     string accessToken,
     CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v2/userinfo",
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BadRequestException($"Failed to get user info: {error}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<GoogleUserDto>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<(UserDto domainUser, Guid identityId)> FindOrCreateGoogleUserAsync(GoogleUserDto googleUser, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(googleUser.Email))
                throw new ArgumentException("Email is required");

            var identityUser = await _userManager.FindByEmailAsync(googleUser.Email);
            User domainUser;

            if (identityUser == null)
            {
                // 1. Tạo DomainUser
                domainUser = new User(
                    fullName: googleUser.Name,
                    email: googleUser.Email,
                    phoneNumber: string.Empty,
                    birthDay: null,
                    providerName: "GOOGLE",
                    status: StatusUser.Active
                );

                await _userRepository.AddAsync(domainUser, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // ✅ commit xong

                // 2. Tạo IdentityUser sau khi DomainUser đã tồn tại
                identityUser = new ApplicationUser
                {
                    UserName = googleUser.Email,
                    Email = googleUser.Email,
                    EmailConfirmed = true,
                    UserId = domainUser.Id
                };

                var result = await _userManager.CreateAsync(identityUser);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(identityUser, "User");
            }
            else
            {
                // 3. Lấy DomainUser đã tồn tại
                domainUser = await _userRepository.GetByIdAsync(identityUser.UserId, cancellationToken)
                    ?? throw new Exception($"Domain user not found for UserId: {identityUser.UserId}");
            }

            return (new UserDto
            {
                Id = domainUser.Id,
                Code = domainUser.Code,
                FullName = domainUser.FullName,
                Email = domainUser.Email,
                PhoneNumber = domainUser.PhoneNumber,
                BirthDay = domainUser.BirthDay ?? default,
                ProviderName = domainUser.ProviderName,
                StatusUser = domainUser.StatusUser
            }, identityUser.Id);
        }
        public async Task<AuthResponseDto> GoogleLoginAsync(string code,CancellationToken cancellationToken = default)
        {
            // 1. Exchange code
            var googleToken = await ExchangeGoogleCodeAsync(code, cancellationToken);

            // 2. Get user info
            var googleUser = await GetGoogleUserInfoAsync(
                googleToken.AccessToken,
                cancellationToken);

            // 3. Find or create user
            var (user, identityId) = await FindOrCreateGoogleUserAsync(
                googleUser,
                cancellationToken);
            
            // 4. Get roles
            var roles = await _roleService.GetUserRoles(identityId); 

            // 5. Generate JWT (🔥 quan trọng nhất)
            var tokens = await _jwtTokenGenerator.GenerateTokenAsync(
                user.Id,
                user.Email,
                roles);

            // 6. Save refresh token
            var refreshToken = RefreshToken.Create(user.Id, tokens.RefreshToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Return (🔥 chỉ trả token của hệ thống)
            return new AuthResponseDto
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.ExpiresAt,
                ExpiresIn = tokens.ExpiresIn,
                TokenType = tokens.TokenType
            };
        }
    }


}

