using QueueManagement.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IQueueTicketRepository : IGenericRepository<QueueTicket>,IPaginatedRepository<QueueTicket>
    {
        Task<QueueTicket?> GetTicketWithDetailsAsync(Guid ticketId, CancellationToken cancellationToken = default);
        Task<int> GetTodayTicketCountAsync(CancellationToken cancellationToken = default);
        Task<QueueTicket?> GetCurrentlyCalledTicketAsync(CancellationToken cancellationToken = default);
        Task<List<QueueTicket>> GetAllWaitingTicketsWithServiceAsync(CancellationToken cancellationToken = default);
        Task<List<QueueTicket>> GetWaitingTicketsWithServiceAsync(CancellationToken cancellationToken = default);
    }
}
