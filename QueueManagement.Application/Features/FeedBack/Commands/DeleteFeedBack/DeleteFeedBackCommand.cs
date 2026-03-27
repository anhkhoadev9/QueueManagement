using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.FeedBack.Commands.DeleteFeedBack
{
    public class DeleteFeedBackCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
