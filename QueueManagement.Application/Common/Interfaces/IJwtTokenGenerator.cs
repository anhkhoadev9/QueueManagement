using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<AuthResponseDto> GenerateTokenAsync(Guid userId, string email, IList<string> roles);
    }

}

