using MediatR;
using QueueManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.AddFeedBack
{
    public class AddFeedBackCommand : IRequest<Unit>
    {

        public string? Comment { get; set; }
        public Guid UserId { get; set; }
        public Guid QueueTicketId { get; set; }
        public Guid ServiceId { get; set; }
        public int? Rating { get; set; }


    }
}
