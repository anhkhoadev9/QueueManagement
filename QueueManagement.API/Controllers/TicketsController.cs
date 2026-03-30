using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Features.Tickets.Commands.CallTicket;
using QueueManagement.Application.Features.Tickets.Commands.GenerateTicket;
using QueueManagement.Application.Features.Tickets.Commands.UpdateTicketStatus;
using QueueManagement.Application.Features.Tickets.Queries.GetCurrentlyCalledTicket;
using QueueManagement.Application.Features.Tickets.Queries.GetTicketById;
using QueueManagement.Application.Features.Tickets.Queries.GetWaitingTickets;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Application.Features.Tickets.Commands.SubmitFeedback;
using QueueManagement.Application.Features.Tickets.Queries.GetPaginatedTickets;
using QueueManagement.Application.Features.Tickets.Queries.GetTicketHistoryByTicketId;
using QueueManagement.Application.Features.Tickets.Commands.CallNextTicket;
using QueueManagement.Application.Features.Tickets.Queries.GetAllWattingTickets;

namespace QueueManagement.API.Controllers
{
    public class TicketsController : BaseApiController
    {
        public TicketsController(IMediator mediator) : base(mediator)
        {

        }
        // modify CreatedAtAction and add handle getId
        [HttpPost]
        public async Task<ActionResult> Generate(GenerateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _mediator.Send(command, cancellationToken);
            return Ok(ticket);
            //return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateStatus(UpdateTicketStatusCommand command, CancellationToken cancellation)
        {
            await _mediator.Send(command, cancellation);
            return NoContent();
        }

        [HttpGet("waiting")]
        public async Task<ActionResult<List<TicketDto>>> GetWaitingTickets(
     CancellationToken cancellation)
        {
            // Query không cần tham số - tạo mới trong action
            var query = new GetWaitingTicketsQuery();
            var result = await _mediator.Send(query, cancellation);
            return Ok(result);
        }
        [HttpGet("waitings")]
        public async Task<ActionResult<List<TicketDto>>> GetAllWaitingTickets(
    CancellationToken cancellation)
        {
            // Query không cần tham số - tạo mới trong action
            var query = new GetAllWattingTicketsQuery();
            var result = await _mediator.Send(query, cancellation);
            return Ok(result);
        }

        [HttpGet("current")]
        public async Task<ActionResult<TicketDto>> GetCurrentTicket(
            CancellationToken cancellation)
        {
            var query = new GetCurrentlyCalledTicketQuery();
            var result = await _mediator.Send(query, cancellation);
            return Ok(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TicketDto>> GetTicketById(Guid id)
        {
            var query = new GetTicketByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("call")]
        public async Task<IActionResult> CallTicket(CallTicketCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("call-next")]
        public async Task<ActionResult<TicketDto>> CallNextTicket(CallNextTicketCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{ticketId:guid}/feedback")]
        public async Task<IActionResult> SubmitFeedback([FromRoute] Guid ticketId, [FromBody] SubmitFeedbackCommand command, CancellationToken cancellationToken)
        {
            var propertyInfo = command.GetType().GetProperty("TicketId");
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(command, ticketId);
            }
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedTickets([FromQuery] GetPaginatedTicketsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{ticketId:guid}/history")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] Guid ticketId, [FromQuery] GetTicketHistoryByTicketIdQuery query, CancellationToken cancellationToken)
        {
            var propertyInfo = query.GetType().GetProperty("TicketId");
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(query, ticketId);
            }
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
