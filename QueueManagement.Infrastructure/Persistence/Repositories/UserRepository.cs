using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;

using QueueManagement.Infrastructure.Identity;
using QueueManagement.Infrastructure.Persistence.Context;
namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly QueueManagementDbContext _context;
        public UserRepository(QueueManagementDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {

            _userManager = userManager;
            _context = context;
        }

        public async Task<User> GetInforUser(string infor, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(infor)) throw new InvalidCastException(infor);
                

            infor = infor.Trim();

            // Bước 1: Tìm trong AspNetUsers theo UserName hoặc Email
            var appUser = await _userManager.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName == infor ||
                    u.Email == infor,
                    cancellationToken);

            if (appUser == null) throw new NotFoundException("Not found user");
             

            // Bước 2: Tìm trong bảng User (Domain) theo Id
            var user = await _dbSet
                .FirstOrDefaultAsync(u => u.Id == appUser.UserId, cancellationToken);

            return user;
        }
    }

}
