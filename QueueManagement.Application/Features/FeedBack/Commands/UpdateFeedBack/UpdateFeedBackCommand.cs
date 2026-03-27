using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.UpdateFeedBack
{
    public class UpdateFeedBackCommand:IRequest<Unit>

    {
        public Guid Id { get; set; }
        public string? Comment { get; set; }
        public Guid UserId { get; set; }
        public Guid QueueTicketId { get; set; }
        public int? Rating { get; set; }
    }
}
