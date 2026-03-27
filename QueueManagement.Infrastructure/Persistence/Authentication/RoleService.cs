using Microsoft.AspNetCore.Identity;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Authentication
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public RoleService(  UserManager<ApplicationUser>userManager )
        {
             
            _userManager = userManager;
        }



        public async Task<IList<string>> GetUserRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found");
            }

            // Trả về danh sách tên roles
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }
    }
}
