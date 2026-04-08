using QueueManagement.Application.DTOs;
 

namespace QueueManagement.Application.Common.Interfaces
{
    public interface IQueueHubService
    {
        Task NotifyTicketCreated(TicketDto ticket);
        Task NotifyTicketCalled(TicketDto ticket);
        Task NotifyTicketCompleted(TicketDto ticket);
        Task NotifyQueueUpdated();
    }
}
