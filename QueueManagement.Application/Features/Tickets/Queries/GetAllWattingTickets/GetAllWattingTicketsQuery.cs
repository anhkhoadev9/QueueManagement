using MediatR;
using QueueManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetAllWattingTickets
{
    public class GetAllWattingTicketsQuery: IRequest<List<TicketDto>>
    {
    }
}
