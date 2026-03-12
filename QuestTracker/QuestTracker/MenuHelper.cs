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
                Console.WriteLine("========================================");
                Console.WriteLine("   WELCOME TO THE QUEST GUILD TERMINAL");
                Console.WriteLine("========================================\n");
                
                Console.WriteLine("  1. Register Hero\n");
                Console.WriteLine("  2. Login Hero\n");
                Console.WriteLine("  3. Exit\n");
                
                Console.WriteLine("========================================");
                Console.Write("  Choose an option (1-3): ");


                bool validInput = int.TryParse(Console.ReadLine(), out int choice);

                if (!validInput)
                {
                    Console.WriteLine("\nInvalid choice! Try again.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }


                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        authenticator.Registration();
                        break;

                    case 2:
                        Console.Clear();
                        authenticator.Login();
                        break;

                    case 3:

                        running = false;
                        Console.WriteLine("\nFarewell, brave hero!");
                        break;

                    default:

                        Console.WriteLine("\nInvalid choice! Try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
                Console.WriteLine("========================================");
                Console.WriteLine($"   GUILD HALL - Welcome, {User.Username}!");
                Console.WriteLine("========================================\n");
                
                Console.WriteLine("  1. Add new quest\n");
                Console.WriteLine("  2. View all quests\n"); 
                Console.WriteLine("  3. Update / Complete quest\n");
                Console.WriteLine("  4. Request Guild Advisor help (AI)\n");
                Console.WriteLine("  5. Show guild report\n");
                Console.WriteLine("  6. Logout\n");
                
                Console.WriteLine("========================================");
                Console.Write("  Choose an option (1-6): ");

                bool validInput = int.TryParse(Console.ReadLine(), out int choice);

                if (!validInput)
                {
                    Console.WriteLine("\nInvalid choice! Try again.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }


                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        questmanager.AddQuest();

                        Notifications notify = new Notifications(questmanager);
                        notify.CheckDeadlinesAndNotify(User.PhoneNumber);
                        break;

                    case 2:
                        Console.Clear();
                        questmanager.ShowAllQuests();
                        break;

                    case 3:
                        Console.Clear();
                        questmanager.UpdateQuest();
                        break;

                    case 4:
                        GuildHelperAI helper = new GuildHelperAI();
                        await helper.RunAsync();
                        Console.Clear(); // Rensa efter att AI:n är klar, inte innan
                        break;

                    case 5:
                        Console.Clear();
                        questmanager.ShowReport();
                        break;

                    case 6:

                        loggedIn = false;
                        Console.WriteLine("\nYou have left the Guild Hall.");
                        break;

                    default:

                        Console.WriteLine("\nInvalid choice! Try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
