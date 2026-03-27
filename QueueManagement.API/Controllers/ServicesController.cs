using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Application.Features.Service.Commands.CreatedService;
using QueueManagement.Application.Features.Service.Commands.DeletedService;
using QueueManagement.Application.Features.Service.Commands.ChangeStatusService;
using QueueManagement.Application.Features.Service.Commands.UpdatedService;
using QueueManagement.Application.Features.Service.Queries;
using QueueManagement.Application.Features.Service.Queries.GetIdService;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/services")]
    public class ServicesController : BaseApiController
    {
        public ServicesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreatedServiceCommands command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateService([FromRoute] Guid id, [FromBody] UpdatedServiceCommands command, CancellationToken cancellationToken)
        {
            // If the command's Id has private set, you might need to use reflection or change the DTO to handle it.
            // typeof(UpdatedServiceCommands).GetProperty("Id")?.SetValue(command, id);
            
            // To ensure the ID from route is used, if your command allows setting it:
            var propertyInfo = command.GetType().GetProperty("Id");
            if(propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(command, id);
            }
            
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteService([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeletedServiceCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/lock")]
        public async Task<IActionResult> LockService([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            // Assuming LockServiceCommand has a constructor or you can set properties. Provide a helper method or change private set to set.
            // Here using reflection to circumvent private set issue if it exists
            var command = new LockServiceCommand();
            var propertyInfo = command.GetType().GetProperty("Id");
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(command, id);
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetServiceById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetIdServiceQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedServices([FromQuery] GetPaginatedResultServiceQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
