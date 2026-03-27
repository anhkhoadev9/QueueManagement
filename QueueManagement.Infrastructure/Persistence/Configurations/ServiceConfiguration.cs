using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Persistence.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.EstimatedDurationMinus)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
