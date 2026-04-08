using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class TicketHistoryRepository : GenericRepository<TicketHistory>, ITicketHistoryRepository
    {
        public TicketHistoryRepository(QueueManagementDbContext context) : base(context) { }

        /// <summary>
        /// TODO: implement khi QueueTicket có FK → User.
        /// </summary>
        public Task GetActiveTicketsByUserId(Guid UserId)
        {
            return Task.CompletedTask;
        }
    }
}
