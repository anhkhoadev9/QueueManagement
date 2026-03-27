using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Enum
{
    public enum EmailStatus:byte
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
