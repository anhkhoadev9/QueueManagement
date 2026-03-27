using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Persistence.Configurations
{
    public class QueueTicketConfiguration : IEntityTypeConfiguration<QueueTicket>
    {
        public void Configure(EntityTypeBuilder<QueueTicket> builder)
        {
            builder.ToTable("QueueTickets");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.IssuedAt)
                .IsRequired();

            // Relationship: QueueTicket → Service (many-to-one, no reverse nav collection on Service)
            builder.HasOne(q => q.Service)
                .WithMany()
                .HasForeignKey(q => q.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: QueueTicket → TicketHistories (one-to-many)
            builder.HasMany(q => q.Histories)
                .WithOne(h => h.QueueTicket)
                .HasForeignKey(h => h.QueueTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: QueueTicket → Feedback (one-to-one)
            builder.HasOne(q => q.Feedback)
                .WithOne(f => f.QueueTicket)
                .HasForeignKey<Feedback>(f => f.QueueTicketId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for fast lookup
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.TicketNumber);
            builder.HasIndex(x => x.IssuedAt);
        }
    }
}
