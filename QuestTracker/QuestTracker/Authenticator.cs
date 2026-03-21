using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HeroProject
{
    public class Authenticator
    {

        private List<User> users = new List<User>();
        private string filePath;

        public Authenticator()
        {
            // Set the file path to be in the same directory as the executable
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(baseDir, "users.json");
            
            LoadUsersFromFile();
        }

        //Registration
        public void Registration()
        {
            Console.WriteLine("--- Your adventure begins here! Sign up to track quests ---");

            string username;

            while (true)
            {
                Console.WriteLine("Please write down your username (or type 'back' to return to the menu):");
                username = Console.ReadLine();

                if (username?.ToLower() == "back")
                    return; // Går tillbaka till menyn

                bool usernameExists = users.Any(u => u.Username == username);

                if (usernameExists)
                {
                    Console.WriteLine("This username already exists. Please choose another one.");
                }
                else
                {
                    break;
                }
            }


            Console.WriteLine("Please write down the password (must meet all criteria below):");
            int startLine = Console.CursorTop;
            string password = "";
            bool isStrong = false;
            while (true)
            {
                Console.SetCursorPosition(0, startLine);
                Console.Write("Password: " + new string('*', password.Length).PadRight(20));

                var (len, up, dig, spec) = User.GetCriteria(password);
                isStrong = len && up && dig && spec;

                Console.SetCursorPosition(0, startLine + 1);
                
                Console.ForegroundColor = len ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($" [{(len ? 'v' : ' ')}] 6+ characters  ");
                Console.ForegroundColor = up ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($" [{(up ? 'v' : ' ')}] Uppercase  ");
                Console.ForegroundColor = dig ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($" [{(dig ? 'v' : ' ')}] Number  ");
                Console.ForegroundColor = spec ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($" [{(spec ? 'v' : ' ')}] Special character  ");
                Console.ResetColor();
                Console.Write("     "); // Clear trailing chars

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (isStrong)
                    {
                        Console.WriteLine();
                        break;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                }
            }
            Console.WriteLine();

            Console.WriteLine("Phone number (e.g, +46700000000):");
            string phonenumber = Console.ReadLine();

            User newUser = new User
            {
                Username = username,
                PhoneNumber = phonenumber
            };
            newUser.SetPassword(password);


            users.Add(newUser);
            SaveUsersToFile();

            Console.WriteLine("Registration completed!");


            newUser.ShowProfile();
            Console.ReadKey();
        }

        //Inloggning
        public void Login()
        {

            bool loggedIn = false;


            while (!loggedIn)
            {
                Console.WriteLine("Log in");
                Console.WriteLine("Enter your username (or type 'back' to return to the menu):");
                string Username = Console.ReadLine();

                if (Username?.ToLower() == "back")
                    return; // Återgå till huvudmenyn

                Console.WriteLine("Enter the password:");

                string Password = "";
                ConsoleKeyInfo key;

                while (true)
                {
                    key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && Password.Length > 0)
                    {
                        Password = Password[..^1];
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        Password += key.KeyChar;
                        Console.Write("*");
                    }
                }


                var matchedUser = users.FirstOrDefault(u => u.Username == Username && u.CheckPassword(Password));//LINQ

                if (matchedUser == null)
                {
                    Console.WriteLine("No user found with that username.");
                    Console.WriteLine("Please register first from the main menu.");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    continue;
                }
                else
                {
                    var random = new Random();
                    string code = random.Next(100000, 999999).ToString();

                    SendSms2FA(matchedUser.PhoneNumber, code);

                    int attempts = 3;
                    bool verified = false;

                    while (attempts > 0 && !verified)
                    {
                        Console.WriteLine($"\nVerification code sent! (Attempts left: {attempts})");
                        string inputCode = GetVerificationCodeWithVisuals(6);

                        if (inputCode == code)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"\n\n   Welcome back, {Username}!");
                            Console.ResetColor();
                            Console.WriteLine("   Press any key to enter the Guild Hall...");
                            Console.ReadKey();
                            
                            verified = true;
                            loggedIn = true;
                            MenuHelper.LoggedInMenu(matchedUser);
                        }
                        else
                        {
                            attempts--;
                            if (attempts > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n\n   Incorrect verification code. Please try again.");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n\n   Too many failed attempts. Security lock engaged.");
                                Console.ResetColor();
                                Console.WriteLine("   Returning to main menu...");
                                Console.ReadKey();
                                return; // Back to start menu
                            }
                        }
                    }
                }
            }
        }

        private string GetVerificationCodeWithVisuals(int length)
        {
            string input = "";
            int startX = Console.CursorLeft + 3;
            int startY = Console.CursorTop + 1;

            while (input.Length < length || true)
            {
                // Draw Squares
                Console.SetCursorPosition(startX, startY);
                for (int i = 0; i < length; i++)
                {
                    if (i < input.Length)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"[{input[i]}] ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("[ ] ");
                    }
                }
                Console.ResetColor();

                if (input.Length == length) break;

                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                }
                else if (char.IsDigit(key.KeyChar) && input.Length < length)
                {
                    input += key.KeyChar;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return "";
                }
            }

            return input;
        }


        private void SendSms2FA(string phoneNumber, string code)
        {

            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
            {
                Console.Error.WriteLine("Missing TWILIO_ACCOUNT_SID or TWILIO_AUTH_TOKEN.");
                Environment.Exit(1);
                return;
            }


            TwilioClient.Init(accountSid, authToken);


            var from = new PhoneNumber("+17164665965");
            var to = new PhoneNumber(phoneNumber);


            var msg = MessageResource.Create(
                to: to,
                from: from,
                body: $"Your 2FA code is: {code}"
            );


            Console.WriteLine($"2FA code sent to {phoneNumber}");
        }


        private void SaveUsersToFile()
        {
            string json = JsonSerializer.Serialize(users);
            File.WriteAllText(filePath, json);
        }


        private void LoadUsersFromFile()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }
    }
}


