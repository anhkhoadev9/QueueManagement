using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Persistence.Configurations
{
    public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.ToTable("TicketHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OldStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.NewStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ChangedBy)
                .HasMaxLength(100);

            builder.Property(x => x.ChangedAt)
                .IsRequired();
        }
    }

    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(1000);

            builder.Property(x => x.SubmittedAt)
                .IsRequired();
        }
    }
}
