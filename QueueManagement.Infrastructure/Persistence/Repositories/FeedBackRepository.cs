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

        

    }
}
