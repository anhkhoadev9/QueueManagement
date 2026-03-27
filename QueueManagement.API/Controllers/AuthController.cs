using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueManagement.API.Middlewares;
using QueueManagement.Application.Features.Auth.Commands.ChangePassword;
using QueueManagement.Application.Features.Auth.Commands.ForgotPassword;
using QueueManagement.Application.Features.Auth.Commands.Login;
using QueueManagement.Application.Features.Auth.Commands.Logout;
using QueueManagement.Application.Features.Auth.Commands.RefreshToken;
using QueueManagement.Application.Features.Auth.Commands.Register;
using QueueManagement.Application.Features.Auth.Commands.Revoked;
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
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginCommand request, CancellationToken cancellation)
        {
            var login = await _mediator.Send(request, cancellation);
            return Ok(login);

        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromQuery] RegisterCommand request, CancellationToken cancellationToken)
        {
            request.DateOfBirth = DateTime.TryParse(request.DateOfBirth.ToString(), out var dob) ? dob : request.DateOfBirth;
            var register = await _mediator.Send(request, cancellationToken);
            return Ok(register);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutCommand request, CancellationToken cancellation)
        {
            await _mediator.Send(request, cancellation);
            return NoContent();
        }
 
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenDto>> RefreshToken([FromBody] RefreshTokenCommand request, CancellationToken cancellation)
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

    }
}




