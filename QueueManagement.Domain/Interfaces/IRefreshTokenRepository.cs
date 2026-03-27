using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IRefreshTokenRepository:IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string refreshtoken,CancellationToken cancellation);
        Task<bool> RevokedAllAsync(Guid userId, CancellationToken cancellation);
    }
}
