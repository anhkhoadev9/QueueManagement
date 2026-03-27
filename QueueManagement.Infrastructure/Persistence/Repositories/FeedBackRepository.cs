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

        //public async Task<List<Feedback>> GetByQueueTicketIdAsync(Guid queueTicketId)
        //{
        //    return await _context.Feedbacks
        //        .Where(f => f.QueueTicketId == queueTicketId)
        //        .ToListAsync();
        //}
        //public async Task<Feedback?> GetByIdAsync(Guid id)
        //{
        //    return await _context.Feedbacks.Where(f => f.Id == id).FirstOrDefaultAsync();
        //}

    }
}
