using QueueManagement.Application.Common.Interfaces;
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
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        
        public ServiceRepository(QueueManagementDbContext context) : base(context) { }
    }

}
