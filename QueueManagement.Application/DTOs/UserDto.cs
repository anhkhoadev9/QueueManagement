using QueueManagement.Domain.Enum;
using System;

namespace QueueManagement.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public StatusUser StatusUser { get; set; }
        public string ProviderName { get; set; }
    }
}
