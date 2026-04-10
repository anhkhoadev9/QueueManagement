using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Queries.GetRole
{
    public class GetRoleQuery: IRequest<string>
    {
        public Guid IdentityUserId { get; set; }    

    }
}
