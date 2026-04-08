using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.DTOs
{
    public class FeedbackDto
    {
        public Guid Id { get; set; }
        public Guid QueueTicketId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Optional: thông tin thêm
        public string? TicketNumber { get; set; }
        public string? CustomerName { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
    }
}
