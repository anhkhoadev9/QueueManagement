using MediatR;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Audit
{
    // ========================
    // DTO
    // ========================
    public record DailyReportDto(
        DateTime Date,
        int TotalTickets,
        int WaitingTickets,
        int CompletedTickets,
        int CancelledTickets,
        double AverageWaitingMinutes
    );

    // ========================
    // Query
    // ========================
    public record GetDailyReportQuery(DateTime? Date = null) : IRequest<DailyReportDto>;

    // ========================
    // Handler
    // ========================
    public class GetDailyReportQueryHandler : IRequestHandler<GetDailyReportQuery, DailyReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDailyReportQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DailyReportDto> Handle(GetDailyReportQuery request, CancellationToken cancellationToken)
        {
            var reportDate = (request.Date ?? DateTime.UtcNow).Date;
            var nextDay = reportDate.AddDays(1);

            // Fetch all tickets for the day
            var tickets = await _unitOfWork.QueueTicketRepository.GetAsync(
                t => t.IssuedAt >= reportDate && t.IssuedAt < nextDay
            );

            int totalTickets    = tickets.Count;
            int waitingTickets  = tickets.Count(t => t.Status == TicketStatus.Waiting);
            int completedTickets= tickets.Count(t => t.Status == TicketStatus.Completed);
            int cancelledTickets= tickets.Count(t => t.Status == TicketStatus.Cancelled);

            // Average wait time = from IssuedAt → CalledAt for tickets that were called
            var ticketsWithCallTime = tickets.Where(t => t.CalledAt.HasValue).ToList();

            double averageWaitingMinutes = ticketsWithCallTime.Any()
                ? ticketsWithCallTime.Average(t => (t.CalledAt!.Value - t.IssuedAt).TotalMinutes)
                : 0;

            return new DailyReportDto(
                Date: reportDate,
                TotalTickets: totalTickets,
                WaitingTickets: waitingTickets,
                CompletedTickets: completedTickets,
                CancelledTickets: cancelledTickets,
                AverageWaitingMinutes: Math.Round(averageWaitingMinutes, 2)
            );
        }
    }
}
