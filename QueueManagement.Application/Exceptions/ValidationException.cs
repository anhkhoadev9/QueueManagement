using System;
using System.Collections.Generic;

using System.Collections.Generic;
using System.Net;

namespace QueueManagement.Application.Exceptions
{
    public class ValidationException : CustomException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : this()
        {
            Errors = errors;
        }
    }
}
