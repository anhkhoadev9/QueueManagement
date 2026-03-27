using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Exceptions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace QueueManagement.Domain.Entities
{
    public class TokenResult
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}