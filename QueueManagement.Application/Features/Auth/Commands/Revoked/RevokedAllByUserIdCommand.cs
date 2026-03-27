using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Revoked
{
    public class RevokedAllByUserIdCommand: IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
