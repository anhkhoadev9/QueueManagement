using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Application.Features.FeedBack.Commands.AddFeedBack;
using QueueManagement.Application.Features.FeedBack.Commands.UpdateFeedBack;
using QueueManagement.Application.Features.FeedBack.Commands.DeleteFeedBack;
using QueueManagement.Application.Features.FeedBack.Queries.GetFeedbackByServiceId;
using QueueManagement.Application.Features.FeedBack.Queries.GetPaginateByQueueTicket;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Features.FeedBack.Queries.GetPaginated;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/feedbacks")]
    public class FeedbacksController : BaseApiController
    {
        public FeedbacksController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] AddFeedBackCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateFeedback([FromRoute] Guid id, [FromBody] UpdateFeedBackCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFeedback([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteFeedBackCommand { Id = id };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpGet("service/{serviceId:guid}")]
        public async Task<IActionResult> GetFeedbackByServiceId([FromRoute] Guid serviceId, [FromQuery] GetFeedbackByServiceIdQuery query, CancellationToken cancellationToken)
        {
            query.ServiceId = serviceId;
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{QueueTicketID:guid}")]
        public async Task<IActionResult> GetPaginatedFeedback([FromRoute] GetPaginateFeedBackByQueueTicketQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<FeedbackDto>>>GetPaginated([FromQuery] GetPaginatedQuery query, CancellationToken cancellationToken)
        {

            var result= await _mediator.Send(query,cancellationToken);
            return Ok(result);
        }
    }
}
