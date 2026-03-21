using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace HeroProject
{
    public class QuestManager
    {
        private string connectionString = @"Server=.\SQLEXPRESS;Database=QuestDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private List<Quest> quests = new List<Quest>();


        public void AddQuest()
        {
            Quest newQuest = new Quest();
            newQuest.CreateAQuest();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO dbo.Quests (Title, Description, Status, DueDate, Priority) VALUES (@title, @desc, @status, @due, @priority)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@title", newQuest.Name);
                    command.Parameters.AddWithValue("@desc", newQuest.Description);
                    command.Parameters.AddWithValue("@status", newQuest.Status.ToString());
                    command.Parameters.AddWithValue("@due", newQuest.DueDate);
                    command.Parameters.AddWithValue("@priority", newQuest.Priority);
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("\nQuest successfully created and saved to database!");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }



        public void ShowAllQuests()
        {
            List<Quest> sqlQuests = GetAllQuestsFromDb();

            if (sqlQuests.Count == 0)
            {
                Console.WriteLine("There are no missions.");
            }
            else
            {
                foreach (var quest in sqlQuests)
                {
                    Console.WriteLine("\n----------------------------");
                    quest.ShowQuestInfo();
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }




        public void CompleteQuest()
        {
            Console.Write("Please Enter the ID of the quest to complete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE dbo.Quests SET Status = @status WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@status", QuestStatus.Completed.ToString());
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("The mission is marked as completed in the database!");
                    else
                        Console.WriteLine("No mission found with that ID.");
                }
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



        public void UpdateQuest()
        {
            Console.Write("Write down the ID for the mission you want to change the status: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Console.WriteLine("Please write down the status for the mission (ongoing/completed/failed/waiting):");
            string statusInput = Console.ReadLine().ToLower();
            QuestStatus newStatus;

            switch (statusInput)
            {
                case "ongoing": newStatus = QuestStatus.Ongoing; break;
                case "completed": newStatus = QuestStatus.Completed; break;
                case "failed": newStatus = QuestStatus.Failed; break;
                case "waiting": newStatus = QuestStatus.Waiting; break;
                default:
                    Console.WriteLine("Invalid status. Nothing was changed.");
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                    return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE dbo.Quests SET Status = @status WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@status", newStatus.ToString());
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Status updated in the database!");
                    else
                        Console.WriteLine("No mission found with that ID.");
                }
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }



        public void ShowReport()
        {
            int completed = 0;
            int others = 0;

            List<Quest> sqlQuests = GetAllQuestsFromDb();

            foreach (var q in sqlQuests)
            {
                if (q.Status == QuestStatus.Completed)
                    completed++;
                else
                    others++;
            }


            Console.WriteLine("Guild Report:");
            Console.WriteLine("Number of completed missions: " + completed);
            Console.WriteLine("Number of other missions: " + others);

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }


        private List<Quest> GetAllQuestsFromDb()
        {
            List<Quest> questList = new List<Quest>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Title, Description, Status, DueDate, Priority FROM dbo.Quests";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Quest q = new Quest(reader["Title"].ToString(), reader["Description"].ToString())
                        {
                            ID = (int)reader["Id"],
                            DueDate = reader["DueDate"] != DBNull.Value ? (DateTime)reader["DueDate"] : DateTime.Now,
                            Priority = reader["Priority"] != DBNull.Value ? reader["Priority"].ToString() : "Medium"
                        };

                        if (reader["Status"] != DBNull.Value && Enum.TryParse(reader["Status"].ToString(), out QuestStatus status))
                        {
                            q.Status = status;
                        }

                        questList.Add(q);
                    }
                }
            }
            return questList;
        }

        public List<Quest> GetAllQuests()
        {
            return GetAllQuestsFromDb();
        }
    }
}
