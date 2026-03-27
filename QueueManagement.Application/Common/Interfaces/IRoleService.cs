using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Interfaces
{
    public interface IRoleService
    {
        Task<IList<string>> GetUserRoles(Guid userId);
    }
}
