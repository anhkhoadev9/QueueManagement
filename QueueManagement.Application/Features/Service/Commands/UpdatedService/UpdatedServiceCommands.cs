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
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public int EstimatedDurationMinus { get; private set; } // Thời gian ước tính (phút)
        public bool IsActive { get; private set; }
    }
}
