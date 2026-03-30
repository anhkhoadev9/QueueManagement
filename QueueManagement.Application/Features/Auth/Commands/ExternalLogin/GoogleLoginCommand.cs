using MediatR;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.ExternalLogin
{
    public class GoogleLoginCommand:IRequest<AuthResponseDto>
    {
       public string code{get;set;}
    }
}
