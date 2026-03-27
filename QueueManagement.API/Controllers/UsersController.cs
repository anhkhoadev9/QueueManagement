using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Features.Users.Commands.CreateUser;
using QueueManagement.Application.Features.Users.Commands.DeleteUser;
using QueueManagement.Application.Features.Users.Commands.UpdateUser;
using QueueManagement.Application.Features.Users.Queries.GetPaginatedUsers;
using QueueManagement.Application.Features.Users.Queries.GetUserById;
using QueueManagement.Domain.Entities.Abstractions;
using System;
using System.Threading.Tasks;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : BaseApiController
    {
        public UsersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedUsers([FromQuery] GetPaginatedUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return NoContent();
        }
    }
}
