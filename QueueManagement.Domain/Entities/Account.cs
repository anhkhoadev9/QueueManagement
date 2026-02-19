using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        //Phải kèm ID khóa ngoại và Navigation
        public Guid UserId { get; set; }
        //Navigation
        public User User { get; set; } = null!;

        public StatusAccount StatusAccount { get; set; }
        public Account() { }

        public Account(string username, string passwordHash, Guid userId)
        {
            Code = GenerateCode();
            UserName = username;
            PasswordHash = passwordHash;
            UserId = userId;
            StatusAccount = StatusAccount.Active;
        }
        public void Lock()
        {
            if (StatusAccount != StatusAccount.Active)

                throw new DomainExceptions("Chỉ có thể khóa tài khoản đang hoạt động");
            StatusAccount = StatusAccount.Locked;



        }

        private string GenerateCode()
        {
            return $"ACC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
