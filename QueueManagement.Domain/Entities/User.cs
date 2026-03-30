using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Code { get; private set; } = null!;
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PhoneNumber { get; private set; }
        public DateTime? BirthDay { get; private set; }
        public StatusUser StatusUser { get; private set; }
        public Guid ProviderId { get; private set; }
        public string ProviderName { get; private set; } = null!;

        public List<RefreshToken> RefreshToken { get; set; } = new();
        public Guid IdentityUserId { get; private set; }


        private static readonly Random _random = new();
        public User() { }
        public User(string fullName, string email, string phoneNumber, DateTime? birthDay, string? providerName=null, StatusUser status = StatusUser.Pending)
        {
            Id = Guid.NewGuid();
            Code = GenerateCode(fullName);
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            BirthDay = birthDay;
            ProviderName = string.IsNullOrEmpty(providerName) ? "LOCAL" : providerName;
            StatusUser = status;

        }

        public void UpdateProfile(string fullName, string email, string phoneNumber, DateTime? birthDay, StatusUser status)
        {

            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            if (birthDay.HasValue)
            {
                BirthDay = birthDay.Value;
            }
            StatusUser = status;

        }

        public string GenerateCode(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            // Tách các từ
            var words = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string code = "";

            foreach (var word in words)
            {
                code += word[0]; // lấy chữ cái đầu
            }

            // Thêm số random 3 chữ số

            code += _random.Next(100, 999);

            return code.ToUpper();
        }

        public void Approve()
        {
            if (StatusUser != StatusUser.Pending)
                throw new Exception("User không ở trạng thái chờ duyệt");

            StatusUser = StatusUser.Active;
        }
        public void Lock()
        {
            if (StatusUser == StatusUser.Locked)
                throw new Exception("User đã bị khóa");

            StatusUser = StatusUser.Locked;
        }

    }

}
