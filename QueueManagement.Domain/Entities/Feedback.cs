using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Exceptions;
using System;

namespace QueueManagement.Domain.Entities
{
    public class Feedback : BaseEntity
    {
        public Guid QueueTicketId { get; private set; }
        public QueueTicket? QueueTicket { get; private set; }
        public Guid ServiceId { get; private set; }
        public Service? Service { get; private set; }

        public Guid? UserId { get; private set; }
        public User? User { get; private set; }
        public int Rating { get; private set; } // 1 - 5 stars
        public string? Comment { get; private set; }
        public DateTime SubmittedAt { get; private set; }

        public Feedback() { }

        public Feedback(Guid queueTicketId, Guid serviceId,Guid userId, int rating, string? comment)
        {
            QueueTicketId = queueTicketId;
            ServiceId = serviceId;
            UserId = userId;
            Rating = rating;
            Comment = comment;
            SubmittedAt = DateTime.UtcNow;

        }

        public void EditFeedback(int rating, string? comment)
        {
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;


        }
        public void SoftDeleteFeedback()
        {
            if (IsDelete == true)
                throw new DomainExceptions("Service already deleted!");
            IsDelete = true;
            DeletebAt = DateTime.UtcNow;
        }
    }
}
