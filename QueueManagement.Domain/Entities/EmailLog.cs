using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities
{
    public class EmailLog: BaseEntity
    {
 
        public string ToEmail { get; set; } = null!;
        public string Subject { get; set; } = null!;

        public string? Body { get; set; }

        public EmailStatus Status { get; set; } // Pending, Success, Failed

   
        public DateTime? SentAt { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
