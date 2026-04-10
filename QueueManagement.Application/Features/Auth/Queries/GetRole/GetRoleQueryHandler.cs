using MediatR;
using Microsoft.AspNetCore.Identity;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Queries.GetRole
{
    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, string>
    {
  private readonly IRoleService _roleService;
        public GetRoleQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<string> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetUserRoles(request.IdentityUserId);
            if (roles == null || !roles.Any())
                throw new NotFoundException("Role not found for user");

            return roles.First(); 
        

    }
    }
}
