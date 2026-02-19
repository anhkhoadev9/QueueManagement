using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Enum
{
    public enum StatusAccount : byte
    {
        [EnumMember(Value = "Hoạt động")]
        Active = 1,
        [EnumMember(Value = "Ngưng hoạt động")]
        Locked = 2,
    }
}
