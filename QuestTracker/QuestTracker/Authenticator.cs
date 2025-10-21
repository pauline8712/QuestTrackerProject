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
        private string filePath = "users.json";

        public Authenticator()
        {
            LoadUsersFromFile();
        }

        //Registration
        public void Registration()
        {
            Console.WriteLine("--- Your adventure begins here! Sign up to track quests ---");


            Console.WriteLine("Please write down the username:");
            string username = Console.ReadLine();


            bool usernameExists = users.Any(u => u.Username == username);
            if (usernameExists)
            {
                Console.WriteLine("Username already exists. Please try a different one.");
                return;
            }


            Console.Write("Please write down the password: ");
            string password = "";
            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();



            Console.WriteLine("Phone number (e.g, +46700000000):");
            string phonenumber = Console.ReadLine();


            User newUser = new User
            {
                Username = username,
                Password = password,
                PhoneNumber = phonenumber
            };


            users.Add(newUser);
            SaveUsersToFile();

            Console.WriteLine("Registration completed!");


            newUser.ShowProfile();
        }

        //Inloggning
        public void Login()
        {

            bool loggedIn = false;


            while (!loggedIn)
            {
                Console.WriteLine("Log in");
                Console.WriteLine("Enter your username:");
                string Username = Console.ReadLine();

                Console.WriteLine("Enter the password:");
                string Password = Console.ReadLine();


                var matchedUser = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

                if (matchedUser == null)
                {

                    Console.WriteLine("There is no one with that name, but you can go and register.");
                    Console.WriteLine("Do you want to register now? (yes/no):");
                    string response = Console.ReadLine().ToLower();

                    if (response == "yes")
                    {

                        Registration();

                        Console.WriteLine("Please login with your new credentials.");
                        continue;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {

                    var random = new Random();
                    string code = random.Next(100000, 999999).ToString();


                    SendSms2FA(matchedUser.PhoneNumber, code);


                    Console.Write("Enter the 2FA code sent to your phone: ");
                    string inputCode = Console.ReadLine();

                    if (inputCode == code)
                    {

                        Console.WriteLine($"Welcome {Username}!");
                        loggedIn = true;


                        MenuHelper.LoggedInMenu(matchedUser);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect 2FA code. Please try again.");
                    }
                }
            }
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


            var from = new PhoneNumber("+18142921948");
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


