using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IFeedbackRepository:IGenericRepository<Feedback>,IPaginatedRepository<Feedback>
    {
        Task<List<Feedback>> GetByQueueTicketIdAsync(Guid id, CancellationToken cancellationToken = default);
        IQueryable<Feedback> GetFeedbackQueryWithDetails();
    }
}
