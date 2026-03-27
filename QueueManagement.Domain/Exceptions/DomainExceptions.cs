using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Exceptions
{
    public class DomainExceptions : Exception
    {
        public DomainExceptions() : base("Lỗi nghiệp vụ Domain không hợp lệ")
        {
        }
        
        public DomainExceptions(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        public DomainExceptions(string message) : base(message)
        {
        }
    }
}
