using Microsoft.EntityFrameworkCore;
using QueueManagement.Application.Features.Auth.Commands.Revoked;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository: GenericRepository<RefreshToken>,IRefreshTokenRepository
    {
        
        public RefreshTokenRepository(QueueManagementDbContext context):base(context) {
        
        
        
        
        }
        public async Task<RefreshToken>GetByTokenAsync(string refreshtoken, CancellationToken cancellation)
        {
            return await _dbSet.FirstOrDefaultAsync(a=>a.Token==refreshtoken,cancellation);
        }

        public async Task<bool> RevokedAllAsync(Guid userId, CancellationToken cancellation)
        {
             
            var tokens = await _context.RefreshTokens.Where(u=>u.UserId==userId && u.IsRevoked == false).ToListAsync();
            if (!tokens.Any())
                return false;
            foreach (var revoked in tokens)
            {
                revoked.Revoke();
            }
            return true;
        

                   

        }
    }
}
