using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Application.DTOs;

using QueueManagement.Application.Features.FeedBack.Commands.AddFeedBack;
using QueueManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Common.Mapping
{
    public static class FeedBackMapping
    {
        public static Feedback ToEntity(this AddFeedBackCommand feedBackCommand)
        {
            return new Feedback(
            feedBackCommand.QueueTicketId,
                feedBackCommand.ServiceId,
                feedBackCommand.UserId,
                feedBackCommand.Rating ?? 0,
                feedBackCommand.Comment


            );
        }

        public static List<FeedbackDto> ToDtoList(this List<Feedback> feedbacks)
        {
            if (feedbacks == null || !feedbacks.Any())
                return new List<FeedbackDto>();

            return feedbacks.Select(f => new FeedbackDto
            {
                Id = f.Id,
                QueueTicketId = f.QueueTicketId,
                Rating = f.Rating,
                Comment = f.Comment,
                SubmittedAt = f.SubmittedAt,
                UserId = f.UserId?? Guid.Empty,  // ✅ Thêm UserId
                UserName = f.User?.FullName ?? "Khách vãng lai",  // Lấy tên từ User
                // Optional: Load thêm nếu có Include
                TicketNumber = f.QueueTicket?.TicketNumber,
                CustomerName = f.QueueTicket?.CustomerName,
                ServiceName = f.QueueTicket?.Service?.Name ?? string.Empty
            }).ToList();
        }

        //public static PaginatedResult<Feedback>ToPage(this List<Feedback>backMappings)
        //    {
        //        if (backMappings == null || !backMappings.Any())
        //            return new PaginatedResult<Feedback>();
        //        return backMappings.Select(a=>new )
        //    }
        //}
    }
}