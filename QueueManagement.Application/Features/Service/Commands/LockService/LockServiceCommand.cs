using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.ChangeStatusService
{
    public class LockServiceCommand : IRequest<Unit>
    {
        public Guid Id { get; private set; }

    }
}
