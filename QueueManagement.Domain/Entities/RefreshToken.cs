using QueueManagement.Domain.Entities.Abstractions;

namespace QueueManagement.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string Token { get; private set; } = null!;
        public DateTime ExpiryDate { get; private set; }

        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }


        private RefreshToken() { }

        private RefreshToken(Guid userId, string token, DateTime expiryDate)
        {
            UserId = userId;
            Token = token;
            ExpiryDate = expiryDate;
            IsRevoked = false;
        }


        public static RefreshToken Create(Guid userId, string token, int expiryDays = 7)
        {
            return new RefreshToken(
                userId,
                token,
                DateTime.UtcNow.AddDays(expiryDays)
            );
        }


        public bool IsValid()
        {
            return !IsRevoked && DateTime.UtcNow <= ExpiryDate;
        }


        public void Revoke()
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token already revoked");

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiryDate;
        }
    }
}