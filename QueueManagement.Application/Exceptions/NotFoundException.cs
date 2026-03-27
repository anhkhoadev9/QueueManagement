using System;

using System.Net;

namespace QueueManagement.Application.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string name)
           : base($"Entity \"{name}\" was not found.", HttpStatusCode.NotFound)
        {
        }
    }
}
