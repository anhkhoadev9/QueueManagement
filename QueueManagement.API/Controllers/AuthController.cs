using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueManagement.API.Middlewares;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Features.Auth.Commands.ChangePassword;
using QueueManagement.Application.Features.Auth.Commands.ExternalLogin;
using QueueManagement.Application.Features.Auth.Commands.ForgotPassword;
using QueueManagement.Application.Features.Auth.Commands.Login;
using QueueManagement.Application.Features.Auth.Commands.Logout;
using QueueManagement.Application.Features.Auth.Commands.RefreshToken;
using QueueManagement.Application.Features.Auth.Commands.Register;
using QueueManagement.Application.Features.Auth.Commands.Revoked;
using QueueManagement.Application.Features.Auth.Queries.GetRole;
using QueueManagement.Domain.Entities.DTOs;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(IMediator mediator,IHttpContextAccessor httpContextAccessor) : base(mediator)
        {
            _httpContextAccessor= httpContextAccessor;

        }
        //[HttpPost("login")]
        //public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginCommand request, CancellationToken cancellation)
        //{
        //    var loginResult = await _mediator.Send(request, cancellation);

        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true, 
        //        SameSite = SameSiteMode.None, 
        //        Expires = loginResult.ExpiresAt
        //    };

        //    Response.Cookies.Append("accessToken", loginResult.AccessToken, cookieOptions);
        //    Response.Cookies.Append("refreshToken", loginResult.RefreshToken, cookieOptions);

        //    loginResult.AccessToken = "";
        //    loginResult.RefreshToken = "";

        //    return Ok(loginResult);
        //}
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginCommand request, CancellationToken cancellation)
        {
            var loginResult = await _mediator.Send(request, cancellation);

            // ✅ Xóa cookie cũ ngay từ đầu
            Response.Cookies.Delete("refresh_token");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = loginResult.ExpiresAt
            };

            Response.Cookies.Append("accessToken", loginResult.AccessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", loginResult.RefreshToken, cookieOptions);

            loginResult.AccessToken = "";
            loginResult.RefreshToken = "";

            return Ok(loginResult);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromQuery] RegisterCommand request, CancellationToken cancellationToken)
        {
            request.DateOfBirth = DateTime.TryParse(request.DateOfBirth.ToString(), out var dob) ? dob : request.DateOfBirth;
            var register = await _mediator.Send(request, cancellationToken);
            return Ok(register);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellation)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var command = new LogoutCommand { RefreshToken = refreshToken };
                await _mediator.Send(command, cancellation);
            }
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("refresh_token");
            return NoContent();
        }
 
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand request, CancellationToken cancellation)
        {
            var result = await _mediator.Send(request, cancellation);
            return Ok(result);
        }

        [HttpPost("revoked-all-byUserId/{UserId:Guid}")]
        public async Task<IActionResult> RevokedAllUserIdAsync([FromRoute] RevokedAllByUserIdCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult>ForgotPasswordAsync([FromBody] ForgotPasswordCommand request, CancellationToken cancellation)
        {
            var result=await _mediator.Send(request, cancellation);
            return Ok(result);
        }
        //[Authorize(Roles = "Users")]
        [HttpPost("change-password")]
        public async Task<IActionResult>ChangePasswordAsync([FromBody]ChangePasswordCommand request, CancellationToken cancellation)
        {
             
          
            await _mediator.Send(request, cancellation);
            return NoContent();
        }
        [HttpPost("google-callback")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> GoogleLogin(
            [FromBody] GoogleLoginCommand command)
        {
            var result = await _mediator.Send(command);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = result.ExpiresAt
            };

            Response.Cookies.Append("accessToken", result.AccessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

            result.AccessToken = "";
            result.RefreshToken = "";

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMeAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            if (role == null) return NotFound();
            var query = new QueueManagement.Application.Features.Users.Queries.GetUserById.GetUserByIdQuery { Id = userId };
            var user = await _mediator.Send(query);
            if (user == null) return NotFound();
            var userDto = new UserDto
            {
                Id = user.Id,
                Code = user.Code,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDay = user.BirthDay,
                StatusUser = user.StatusUser,
                ProviderName = user.ProviderName,
                Role = role
            };
            return Ok(userDto);
        }

    }
}




