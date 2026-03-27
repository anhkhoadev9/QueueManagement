using System;
using System.Net;

namespace QueueManagement.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public int StatusCode { get; }

        public UnauthorizedException()
            : base("Unauthorized")
        {
            StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        public UnauthorizedException(string message)
            : base(message)
        {
            StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}