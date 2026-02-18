using QueueManagement.Domain.Interfaces.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.Abstractions
{
    public abstract class Auditable : IAuditableEntities, ISoftDelete
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public string? ReasonForModification { get; set; }
        public bool IsDelete { get; set; }
        public Guid DeletedBy { get; set; }
        public DateTime DeletebAt { get; set; }
        public string? ReasonDeleted { get; set; }
    }
}
