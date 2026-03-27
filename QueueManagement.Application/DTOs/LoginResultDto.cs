using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.DTOs
{
    public class LoginResultDto
    {
        public Guid UserId { get; set; }
        public string IdentityId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
