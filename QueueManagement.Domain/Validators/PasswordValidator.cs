using QueueManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Validators
{
    public class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            return Validate(password).IsValid;
        }

        /// <summary>
        /// Validate password và trả về kết quả chi tiết
        /// </summary>
        public static (bool IsValid, List<string> Errors) Validate(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("Password is required.");
                return (false, errors);
            }

            // 1. Kiểm tra độ dài tối thiểu 8 ký tự
            if (password.Length < 8)
            {
                errors.Add("Passwords must be at least 8 characters.");
            }

            // 2. Kiểm tra có ít nhất 1 chữ số
            if (!password.Any(char.IsDigit))
            {
                errors.Add("Passwords must have at least one digit ('0'-'9').");
            }

            // 3. Kiểm tra có ít nhất 1 chữ thường
            if (!password.Any(char.IsLower))
            {
                errors.Add("Passwords must have at least one lowercase letter ('a'-'z').");
            }

            // 4. Kiểm tra có ít nhất 1 chữ hoa
            if (!password.Any(char.IsUpper))
            {
                errors.Add("Passwords must have at least one uppercase letter ('A'-'Z').");
            }

            // 5. Kiểm tra có ít nhất 1 ký tự đặc biệt (không phải chữ và số)
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errors.Add("Passwords must have at least one non alphanumeric character.");
            }

            return (errors.Count == 0, errors);
        }

        /// <summary>
        /// Validate và throw exception nếu không hợp lệ
        /// </summary>
        public static void ValidateOrThrow(string password)
        {
            var (isValid, errors) = Validate(password);
            if (!isValid)
            {
                throw new DomainExceptions(string.Join(" ", errors));
            }
        }
    }
}
