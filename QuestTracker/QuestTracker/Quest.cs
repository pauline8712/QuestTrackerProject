using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroProject
{
    public enum QuestStatus
    {
        Ongoing,
        Completed,
        Failed,
        Waiting
    }
    public class Quest
    {
        public string Title;
        public int ID;
        public string Description;
        public DateTime DueDate;
        public string Priority;
        public QuestStatus Status;


        public void CreateAQuest()
        {

            Console.WriteLine("Enter the title of the mission:");
            Title = Console.ReadLine();

            Console.Write("Enter an ID: ");
            ID = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the description:");
            string Description = Console.ReadLine();

            Console.WriteLine("When is it due? (YYYY-MM-DD):");
            DateTime dateOnly = DateTime.Parse(Console.ReadLine());
            DueDate = dateOnly.AddHours(23).AddMinutes(59);

            Console.WriteLine("Whats the missions priority? (High/Low):");
            string Priority = Console.ReadLine();


            Status = QuestStatus.Waiting;
        }

        public void ShowQuestInfo()
        {
            Console.WriteLine($"Mission: {Title}");
            Console.WriteLine($"{Description}");
            Console.WriteLine($"{DueDate}");
            Console.WriteLine($"{Priority}");
            Console.WriteLine($"Status: {Status}");
        }
    }
}
