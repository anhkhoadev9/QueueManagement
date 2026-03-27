using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Identity;
using QueueManagement.Infrastructure.Persistence.Context;

public class UnitOfWork : IUnitOfWork
{
    private readonly QueueManagementDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private IDbContextTransaction _currentTransaction;

    public IQueueTicketRepository QueueTicketRepository { get; }
    public IServiceRepository ServiceRepository { get; }
    public ITicketHistoryRepository TicketHistoryRepository { get; }
    public IFeedbackRepository FeedbackRepository { get; }
    public IUserRepository UserRepository { get; }
     public IRefreshTokenRepository RefreshTokenRepository { get; }
    public bool HasActiveTransaction => _currentTransaction != null;
    public UnitOfWork(
        QueueManagementDbContext context,
        IQueueTicketRepository queueTicketRepository,
        IServiceRepository serviceRepository,
        ITicketHistoryRepository ticketHistoryRepository,
        IFeedbackRepository feedbackRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        QueueTicketRepository = queueTicketRepository;
        ServiceRepository = serviceRepository;
        TicketHistoryRepository = ticketHistoryRepository;
        FeedbackRepository = feedbackRepository;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
    }

    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
             
            await _currentTransaction?.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _currentTransaction?.RollbackAsync();
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

     
}