using MediatR;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand: IRequest<RefreshTokenDto>
    {
        public string RefreshToken { get; set; }
      
         
    }
}
