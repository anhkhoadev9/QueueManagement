using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Exceptions
{
    public class DomainExceptions : Exception
    {
        public DomainExceptions() : base()
        {
            throw new Exception("Lỗi khong hợp lệ");
        }
        public DomainExceptions(string message, Exception innerException) : base(message)
        {
            throw new Exception(message, innerException);
        }
        public DomainExceptions(string message) : base(message)
        {
            throw new Exception(message);
        }
    }
}
