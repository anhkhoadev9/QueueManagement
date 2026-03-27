using MediatR;
using QueueManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetTicketById
{
    public class GetTicketByIdQuery : IRequest<TicketDto>
    {
        public Guid Id { get; set; }
    }
}
