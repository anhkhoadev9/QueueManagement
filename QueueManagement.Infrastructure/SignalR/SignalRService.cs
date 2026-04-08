

using Microsoft.AspNetCore.SignalR;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Application.DTOs;
namespace QueueManagement.Infrastructure.SignalR
{
    public class SignalRService : IQueueHubService
    {
        private readonly IHubContext<QueueHub> _hubContext;

        public SignalRService(IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTicketCreated(TicketDto ticket)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTicketCreated", ticket);
        }

        public async Task NotifyTicketCalled(TicketDto ticket)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTicketCalled", ticket);
        }

        public async Task NotifyTicketCompleted(TicketDto ticket)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTicketCompleted", ticket);
        }

        public async Task NotifyQueueUpdated()
        {
            await _hubContext.Clients.All.SendAsync("QueueUpdated");
        }
    }
}
