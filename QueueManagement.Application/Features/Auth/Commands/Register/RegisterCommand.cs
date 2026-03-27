using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<Unit>
    {
        // Basic info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }

        // Additional info
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Captcha for bot prevention
        //public string? CaptchaToken { get; set; }


    }
}
