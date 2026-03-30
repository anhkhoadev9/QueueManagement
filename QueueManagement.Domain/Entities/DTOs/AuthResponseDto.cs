using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } // Thời gian sống của access token (giây)
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; } // Thời gian hết hạn cụ thể
         
    }
}
