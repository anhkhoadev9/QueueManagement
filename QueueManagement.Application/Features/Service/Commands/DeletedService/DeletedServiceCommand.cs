using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.DeletedService
{
    public class DeletedServiceCommand:IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
