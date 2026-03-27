using MediatR;
using QueueManagement.Application.Common.Mapping;
using QueueManagement.Application.DTOs;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Tickets.Queries.GetTicketById
{
    public class GetTickedByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetTickedByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.QueueTicketRepository.GetByIdAsync(request.Id);
            if (ticket == null)
            {
                throw new NotFoundException("Not Found Ticket");
            }
            return TicketMapping.EntityToDTO(ticket);

        }
    }
}
