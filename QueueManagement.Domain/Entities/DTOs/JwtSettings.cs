using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.DTOs
{
    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpiryMinutes { get; set; } = 60; // Access Token sống 60 phút
        public int RefreshTokenExpiryDays { get; set; } = 7; // Refresh Token sống 7 ngày
    }
}
