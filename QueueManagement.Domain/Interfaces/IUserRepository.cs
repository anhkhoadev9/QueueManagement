using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>, IPaginatedRepository<User>
    {
        Task<User>GetInforUser(string infor, CancellationToken cancellationToken);
 
    }
}
