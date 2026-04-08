using Microsoft.EntityFrameworkCore;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class FeedBackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {

        public FeedBackRepository(QueueManagementDbContext context) : base(context) { }


        public async Task<List<Feedback>> GetByQueueTicketIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
       .Include(f => f.QueueTicket)
           .ThenInclude(q => q.Service)
       .Include(f => f.User)  // ✅ Include User
       .OrderByDescending(f => f.SubmittedAt)
       .ToListAsync(cancellationToken);
        }

        public IQueryable<Feedback> GetFeedbackQueryWithDetails()
        {
            return _dbSet.AsNoTracking()
                .Include(f => f.User)
                .Include(f => f.Service)
                .Include(f => f.QueueTicket)
                    .ThenInclude(t => t.Service)
                .AsQueryable();
        }
    }
}
