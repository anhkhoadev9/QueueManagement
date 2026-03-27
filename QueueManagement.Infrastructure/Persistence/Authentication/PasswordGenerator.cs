using QueueManagement.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Persistence.Authentication
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly Random _random = new Random();

        public string Generate( )
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";

            var password = new StringBuilder();

            // Đảm bảo có ít nhất 1 ký tự mỗi loại
            password.Append(upperCase[_random.Next(upperCase.Length)]);      // 1 chữ hoa
            password.Append(lowerCase[_random.Next(lowerCase.Length)]);      // 1 chữ thường
            password.Append(digits[_random.Next(digits.Length)]);            // 1 số
            password.Append(specialChars[_random.Next(specialChars.Length)]); // 1 ký tự đặc biệt

            // Thêm 4-8 ký tự ngẫu nhiên để đạt độ dài 8-12
            const string allChars = upperCase + lowerCase + digits + specialChars;
            int remainingLength = _random.Next(4, 9); // Thêm 4-8 ký tự

            for (int i = 0; i < remainingLength; i++)
            {
                password.Append(allChars[_random.Next(allChars.Length)]);
            }

            // Trộn thứ tự các ký tự
            return new string(password.ToString().OrderBy(x => _random.Next()).ToArray());
        }

       
    }
}
 