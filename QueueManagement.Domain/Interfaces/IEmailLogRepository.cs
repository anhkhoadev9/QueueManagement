using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IEmailLogRepository:IGenericRepository<EmailLog>
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
