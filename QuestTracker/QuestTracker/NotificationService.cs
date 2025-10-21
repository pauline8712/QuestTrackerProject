using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HeroProject
{
    public class Notifications
    {

        private QuestManager _questManager;

        public Notifications(QuestManager questManager)
        {
            _questManager = questManager;
        }



        public void CheckDeadlinesAndNotify(string phoneNumber)
        {
            var now = DateTime.Now;

            foreach (var quest in _questManager.GetAllQuests())
            {
                var timeLeft = quest.DueDate - now;

                Console.WriteLine($"{quest.Title}: {timeLeft.TotalHours} hours left");


                if (quest.Status != QuestStatus.Completed && timeLeft.TotalHours > 0 && timeLeft.TotalHours < 24)
                {
                    Console.WriteLine($"Hero, your mission '{quest.Title}' must be finished by tomorrow!");



                    var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                    var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

                    if (!string.IsNullOrWhiteSpace(accountSid) && !string.IsNullOrWhiteSpace(authToken))
                    {
                        TwilioClient.Init(accountSid, authToken);

                        var from = new PhoneNumber("+18142921948");
                        var to = new PhoneNumber(phoneNumber);

                        var message = MessageResource.Create(
                            to: to,
                            from: from,
                            body: $"⚠️ Hero! Your mission '{quest.Title}' is due soon ({quest.DueDate})."
                        );

                        Console.WriteLine($"SMS notification sent to {to}: {message.Sid}");
                    }
                    else
                    {
                        Console.WriteLine("Twilio credentials missing — SMS not sent.");
                    }
                }
            }
        }



        public void ShowNearDeadlineReport()
        {
            var now = DateTime.Now;
            bool found = false;

            Console.WriteLine("Missions with less than 24 hours remaining until the deadline:");


            foreach (var quest in _questManager.GetAllQuests())
            {
                var timeLeft = quest.DueDate - now;

                if (quest.Status != QuestStatus.Completed && timeLeft.TotalHours > 0 && timeLeft.TotalHours < 24)
                {
                    Console.WriteLine($"- {quest.Title} (Deadline: {quest.DueDate})");
                    found = true;
                }
            }


            if (!found)
                Console.WriteLine("No missions near deadline.");
        }
    }
}
