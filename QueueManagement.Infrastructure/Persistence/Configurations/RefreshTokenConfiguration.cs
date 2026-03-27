using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Persistence.Configurations
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Configure the one-to-one relationship from RefreshToken to User
            builder.ToTable("RefreshTokens"); // Giữ tên bảng là RefreshTokens

            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshToken)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.UserId)
                .IsUnique();

            builder.HasIndex(r => r.Token)
                .IsUnique();


        }
    }
}