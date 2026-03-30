using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Infrastructure.Identity;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Context
{
    public class QueueManagementDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public QueueManagementDbContext(DbContextOptions<QueueManagementDbContext> options) : base(options)
        {
        }

        public DbSet<QueueTicket> QueueTickets { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<TicketHistory> TicketHistories { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<EmailLog> EmailLogs { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Auto-apply all IEntityTypeConfiguration<T> classes in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ApplicationUser>(e => e.ToTable("IdentityUsers"));
            modelBuilder.Entity<ApplicationRole>(e => e.ToTable("IdentityRoles"));

            // Auto-apply all IEntityTypeConfiguration<T> classes in this assembly
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditableEntities();
            return base.SaveChanges();
        }

        private void UpdateAuditableEntities()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var systemUserId = System.Guid.Empty; // TODO: Inject ICurrentUserService and replace with real user ID

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = System.DateTime.UtcNow;
                        entry.Entity.UpdatedAt = System.DateTime.UtcNow;
                        if (entry.Entity.CreatedBy == System.Guid.Empty)
                            entry.Entity.CreatedBy = systemUserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = System.DateTime.UtcNow;
                        if (entry.Entity.UpdatedBy == System.Guid.Empty)
                            entry.Entity.UpdatedBy = systemUserId;
                        break;
                }
            }
        }
    }
}
