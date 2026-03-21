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
        public int ID { get; set; } // Auto-incremented by SQL
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }

        public Quest()
        {
            Status = QuestStatus.Waiting;
            DueDate = DateTime.Now.AddDays(7); // Default deadline
            Priority = "Medium";
        }

        public Quest(string name, string description)
        {
            Name = name;
            Description = description;
            Status = QuestStatus.Waiting;
            DueDate = DateTime.Now.AddDays(7); // Default deadline
            Priority = "Medium";
        }

        public void CreateAQuest()
        {
            Console.Write("Write down the Name for the mission: ");
            Name = Console.ReadLine() ?? "Unnamed Mission";

            Console.Write("Write down the Description for the mission: ");
            Description = Console.ReadLine() ?? "No description provided.";

            Console.Write("Write down the Priority (High/Medium/Low): ");
            Priority = Console.ReadLine() ?? "Medium";

            Console.Write("Write down the Deadline (yyyy-mm-dd) or press Enter for default (7 days): ");
            string dateInput = Console.ReadLine();
            if (DateTime.TryParse(dateInput, out DateTime date))
            {
                DueDate = date;
            }
            else
            {
                DueDate = DateTime.Now.AddDays(7);
            }

            Status = QuestStatus.Waiting;
        }

        public void ShowQuestInfo()
        {
            Console.WriteLine("ID: " + ID);
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("Description: " + Description);
            Console.WriteLine("Priority: " + Priority);
            Console.WriteLine("Status: " + Status);
            Console.WriteLine("Deadline: " + DueDate.ToShortDateString());
        }
    }
}