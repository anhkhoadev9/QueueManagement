using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Queries.GetIdService
{
    public class GetIdServiceQuery : IRequest<Domain.Entities.Service>
    {
        public Guid Id { get; set; }
    }
}
