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
    public class TicketHistoryRepository : GenericRepository<TicketHistory>, ITicketHistoryRepository
    {
        public TicketHistoryRepository(QueueManagementDbContext context) : base(context) { }
    }
}
