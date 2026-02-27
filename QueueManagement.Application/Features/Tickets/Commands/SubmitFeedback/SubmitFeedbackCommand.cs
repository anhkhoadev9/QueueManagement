using MediatR;
using System;

namespace QueueManagement.Application.Features.Tickets.Commands.SubmitFeedback
{
    public class SubmitFeedbackCommand : IRequest<bool>
    {
        public Guid TicketId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
