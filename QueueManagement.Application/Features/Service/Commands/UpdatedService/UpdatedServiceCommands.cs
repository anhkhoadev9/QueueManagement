using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Service.Commands.UpdatedService
{
    public class UpdatedServiceCommands : IRequest<Unit>
    {
        public Guid Id { get;  set; }
        public string Name { get;  set; } = null!;
        public string Description { get;  set; } = null!;
        public int EstimatedDurationMinus { get;  set; } // Thời gian ước tính (phút)
        public bool IsActive { get;  set; }
    }
}
