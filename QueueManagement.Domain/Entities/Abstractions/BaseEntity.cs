using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.Abstractions
{
    public class BaseEntity:Auditable
    {
        public Guid Id { get; set; }
    }
}
