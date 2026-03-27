using QueueManagement.Application.Features.Service.Commands.CreatedService;
using QueueManagement.Application.Features.Service.Commands.UpdatedService;
using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Mapping
{
    public static class ServiceMapping
    {

        public static Service ToEntity(this CreatedServiceCommands command)
        {
            return new Service(command.Name, command.Description, command.EstimatedDurationMinus);

        }
         
    }
}

