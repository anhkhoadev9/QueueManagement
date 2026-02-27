using System;

using System.Net;

namespace QueueManagement.Application.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
