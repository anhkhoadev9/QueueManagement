using Microsoft.EntityFrameworkCore;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class QueueTicketRepository : GenericRepository<QueueTicket>, IQueueTicketRepository
    {
        public QueueTicketRepository(QueueManagementDbContext context) : base(context)
        {
        }

        public async Task<QueueTicket?> GetTicketWithDetailsAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Service)
                .Include(q => q.Histories)
                .Include(q => q.Feedback)
                .FirstOrDefaultAsync(q => q.Id == ticketId, cancellationToken);
        }

        public async Task<int> GetTodayTicketCountAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet.CountAsync(q => q.IssuedAt >= today, cancellationToken);
        }

        public async Task<QueueTicket?> GetCurrentlyCalledTicketAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Service)
                .Where(q => q.Status ==  TicketStatus.InProgress)
                .OrderByDescending(q => q.CalledAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<QueueTicket>> GetAllWaitingTicketsWithServiceAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Service)
                .Where(q => q.Status == TicketStatus.Waiting||q.Status==TicketStatus.InProgress|| q.Status == TicketStatus.Skipped)
                .OrderBy(q => q.IssuedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<QueueTicket>> GetWaitingTicketsWithServiceAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(q => q.Service)
                .Where(q => q.Status == TicketStatus.Waiting)
                .OrderBy(q => q.IssuedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<List<QueueTicket>> GetActiveTicketsByUserId(Guid UserId, CancellationToken cancellationToken)
        {
            // Stubbed out as per user request to not use formal relationship for now
            return Task.FromResult(new List<QueueTicket>());
        }
    }
}
