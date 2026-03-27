using QueueManagement.Application.DTOs;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Mapping
{
    public static class TicketMapping
    {
        public static TicketDto EntityToDTO(this QueueTicket queueTicket)
        {
            if (queueTicket == null) throw new NotFoundException("Not found Ticket!");

            return new TicketDto
            {
                Id = queueTicket.Id,
                TicketNumber = queueTicket.TicketNumber,
                CustomerName = queueTicket.CustomerName,
                PhoneNumber = queueTicket.PhoneNumber,
                ServiceName = queueTicket.Service.Name,
                Status = queueTicket.Status,
                IssuedAt = queueTicket.IssuedAt,
                CalledAt = queueTicket.CalledAt
            };


        }
    }
}
