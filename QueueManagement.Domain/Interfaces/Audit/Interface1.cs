using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces.Audit
{
    public interface ISoftDelete
    {
        public bool IsDelete {get; set;}
        public Guid DeletedBy { get; set;}
        public DateTime DeletebAt { get; set;}
        public string? ReasonDeleted {  get; set;}

    }
}
