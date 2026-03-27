using MediatR;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {

        public string LoginInfo { get; set; } = null!;// nhận username hoặc là email từ client
        public string Password { get; set; } = null!;

    }
}
