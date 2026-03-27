using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.DTOs
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string AppPassword { get; set; } = null!;
    }
}
