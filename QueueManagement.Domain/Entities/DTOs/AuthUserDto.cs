using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.DTOs
{
    public class AuthUserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Guid IdentityId { get; set; }
    }
}
