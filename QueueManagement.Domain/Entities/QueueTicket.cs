using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using System;
using System.Collections.Generic;

namespace QueueManagement.Domain.Entities
{
    public class QueueTicket : BaseEntity
    {
        public string TicketNumber { get; private set; } = null!; // Ví dụ: 1001, 1002
        public string CustomerName { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; } = null!;
        
        public TicketStatus Status { get; private set; }
        public DateTime IssuedAt { get; private set; } // Giờ bấm số
        public DateTime? CalledAt { get; private set; } // Giờ được gọi lên ghế
        public DateTime? CompletedAt { get; private set; } // Giờ cắt xong

        // Navigation Properties
        public ICollection<TicketHistory> Histories { get; private set; } = new List<TicketHistory>();
        public Feedback? Feedback { get; private set; }

        public QueueTicket() { }

        public QueueTicket(string ticketNumber, string customerName, string phoneNumber, Guid serviceId)
        {
            TicketNumber = ticketNumber;
            CustomerName = customerName;
            PhoneNumber = phoneNumber;
            ServiceId = serviceId;
            Status = TicketStatus.Waiting;
            IssuedAt = DateTime.UtcNow;
        }

        public void MarkAsCalled()
        {
            if (Status != TicketStatus.Waiting)
                throw new Exception("Ticket must be Called before InProgress.");

            Status = TicketStatus.InProgress;
        }

        public void MarkAsInProgress()
        {
            if (Status != TicketStatus.Waiting)
                throw new Exception("Ticket must be Called before InProgress.");

            Status = TicketStatus.InProgress;
            CalledAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            if (Status != TicketStatus.InProgress)
                 throw new Exception("Ticket cannot be completed from current status.");

            Status = TicketStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
        public void MarkAsSkipped()
        {
            Status = TicketStatus.Skipped;
        }
        public void MarkAsCancelled()
        {
            Status = TicketStatus.Cancelled;
        }
    }
}
