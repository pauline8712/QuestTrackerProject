using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroProject
{
    public class QuestManager
    {
        private List<Quest> quests = new List<Quest>();


        public void AddQuest()
        {

            Quest newQuest = new Quest();

            newQuest.CreateAQuest();
            quests.Add(newQuest);

            Console.WriteLine("The mission is added.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }



        public void ShowAllQuests()
        {

            for (int i = 0; i < quests.Count; i++)
            {
                Console.WriteLine($"\nQuest #{i + 1}:");
                quests[i].ShowQuestInfo();
            }


            if (quests.Count == 0)
            {
                Console.WriteLine("There is no missions.");
                return;
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }




        public void CompleteQuest()
        {
            Console.Write("Please Enter the ID: ");
            int id = int.Parse(Console.ReadLine());


            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].ID == id)
                {
                    quests[i].Status = QuestStatus.Completed;
                    Console.WriteLine("The mission is marked as completed!");
                    return;
                }
            }

            Console.WriteLine("There is no mission with that ID.");
        }



        public void UpdateQuest()
        {
            Console.Write("Write down the ID for the mission you want to change the status: ");
            int id = int.Parse(Console.ReadLine());

            //LINQ
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].ID == id)
                {
                    Console.WriteLine("\nCurrent quest info:");
                    quests[i].ShowQuestInfo();

                    Console.WriteLine("Please write down the status for the mission (ongoing/completed/failed/waiting):");
                    string statusInput = Console.ReadLine().ToLower();


                    switch (statusInput)
                    {
                        case "ongoing":
                            quests[i].Status = QuestStatus.Ongoing;
                            break;

                        case "completed":
                            quests[i].Status = QuestStatus.Completed;
                            break;

                        case "failed":
                            quests[i].Status = QuestStatus.Failed;
                            break;

                        case "waiting":
                            quests[i].Status = QuestStatus.Waiting;
                            break;

                        default:
                            Console.WriteLine("Invalid status. Nothing was changed.");
                            Console.WriteLine("Press any key to return to the menu...");
                            Console.ReadKey();
                            return;
                    }

                    Console.WriteLine("\nUpdated quest info:");
                    quests[i].ShowQuestInfo();

                    Console.WriteLine("Status updated!");
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }
            }


            Console.WriteLine("No task was found with that ID.");

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }



        public void ShowReport()
        {
            int Completed = 0;
            int NotCompleted = 0;


            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i].Status == QuestStatus.Completed)
                    Completed++;
                else
                    NotCompleted++;
            }


            Console.WriteLine("Report:");
            Console.WriteLine("Number of completed missions: " + Completed);
            Console.WriteLine("Number of other missions: " + NotCompleted);

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }



        public List<Quest> GetAllQuests()
        {
            return quests;
        }
    }
}
