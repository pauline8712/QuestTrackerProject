using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HeroProject
{

    public static class MenuHelper
    {

        private static Authenticator authenticator = new Authenticator();
        private static QuestManager questmanager = new QuestManager();


        public static void StartMenu()
        {

            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("WELCOME TO THE QUEST GUILD TERMINAL");
                Console.WriteLine("Choose an option (1-3)");
                Console.WriteLine("1. Register Hero");
                Console.WriteLine("2. Login Hero");
                Console.WriteLine("3. Exit");


                bool validInput = int.TryParse(Console.ReadLine(), out int choice);

                if (!validInput)
                {
                    Console.WriteLine("Invalid choice! Try again.");
                    continue;
                }


                switch (choice)
                {
                    case 1:

                        authenticator.Registration();
                        break;

                    case 2:

                        authenticator.Login();
                        break;

                    case 3:

                        running = false;
                        Console.WriteLine("Farewell, brave hero!");
                        break;

                    default:

                        Console.WriteLine("Invalid choice! Try again.");
                        break;
                }
            }
        }


        public static async Task LoggedInMenu(User User)
        {

            bool loggedIn = true;


            while (loggedIn)
            {
                Console.Clear();

                Console.WriteLine($"Welcome! Hero: {User.Username}");
                Console.WriteLine("Please choose between 1-6:");
                Console.WriteLine("1. Add new quest");
                Console.WriteLine("2. View all quests");
                Console.WriteLine("3. Update / Complete quest");
                Console.WriteLine("4. Request Guild Advisor help (AI)");
                Console.WriteLine("5. Show guild report");
                Console.WriteLine("6. Logout");


                bool validInput = int.TryParse(Console.ReadLine(), out int choice);

                if (!validInput)
                {
                    Console.WriteLine("Invalid choice! Try again.");
                }


                switch (choice)
                {
                    case 1:

                        questmanager.AddQuest();

                        Notifications notify = new Notifications(questmanager);
                        notify.CheckDeadlinesAndNotify(User.PhoneNumber);
                        break;

                    case 2:
                        questmanager.ShowAllQuests();
                        break;

                    case 3:
                        questmanager.UpdateQuest();
                        break;

                    case 4:

                        GuildHelperAI helper = new GuildHelperAI();
                        await helper.RunAsync();
                        break;

                    case 5:
                        questmanager.ShowReport();
                        break;

                    case 6:

                        loggedIn = false;
                        Console.WriteLine("You have left the Guild Hall.");
                        break;

                    default:

                        Console.WriteLine("Invalid choice! Try again.");
                        break;
                }
            }
        }
    }
}
