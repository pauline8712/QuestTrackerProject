using System;
using System.Linq;

namespace HeroProject
{
    //User klass
    public class User
    {
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public void SetPassword(string password)
        {
            PasswordHash = password;
        }

        public bool CheckPassword(string input)
        {
            return !string.IsNullOrEmpty(PasswordHash) && PasswordHash == input;
        }

        public void ShowProfile()
        {
            Console.WriteLine($"Hero Profile: {Username}");

            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                Console.WriteLine($"Phone Number: {PhoneNumber}");
            }
        }

        public static bool IsPasswordStrong(string password)
        {
            var (length, upper, digit, special) = GetCriteria(password);
            return length && upper && digit && special;
        }

        public static (bool Length, bool Upper, bool Digit, bool Special) GetCriteria(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, false, false, false);

            return (
                password.Length >= 6,
                password.Any(char.IsUpper),
                password.Any(char.IsDigit),
                password.Any(c => !char.IsLetterOrDigit(c))
            );
        }
    }
}