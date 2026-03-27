using QueueManagement.Application.Features.Auth.Commands.Register;
using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Mapping
{
    public static class UserMapping
    {
        public static User ToEntity(this RegisterCommand command)
        {
            var birthDateUtc = DateTime.SpecifyKind(command.DateOfBirth, DateTimeKind.Utc);
            return new User(

                command.FullName,
                command.Email,
                command.PhoneNumber,
                birthDateUtc

            );
        }
    }
}
