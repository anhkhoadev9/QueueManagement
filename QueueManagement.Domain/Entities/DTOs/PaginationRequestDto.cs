using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.DTOs
{
    public class PaginationRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeTicketDetails { get; set; } = false;
        public int MaxPageSize { get; set; } = 50;
    }
}
