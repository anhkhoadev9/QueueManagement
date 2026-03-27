using QueueManagement.Domain.Interfaces;

public interface IUnitOfWork
{
    IQueueTicketRepository QueueTicketRepository { get; }
    IServiceRepository ServiceRepository { get; }
    ITicketHistoryRepository TicketHistoryRepository { get; }
    IFeedbackRepository FeedbackRepository { get; }
    IUserRepository UserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}