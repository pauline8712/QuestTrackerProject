using dotenv.net;
using HeroProject;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;  // <-- här är ändringen
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

internal class Program
{
    // Connection string to your SQL Server database
    static string connectionString = @"Server=.\SQLEXPRESS;Database=QuestDB;Trusted_Connection=True;TrustServerCertificate=True;";

    private static void Main(string[] args)
    {
        DotEnv.Load(); // Load .env variables
        Console.Title = "Quest Guild Tracker";
        Console.ForegroundColor = ConsoleColor.Green;

         // --- TESTA SQL CONNECTION ---
        using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
        {
            conn.Open();
            Console.WriteLine("Connection successful!");
        }
        // -----------------------------

        // Start main menu from MenuHelper
        MenuHelper.StartMenu();

        Console.ResetColor();
    }

    // New method to create a quest via user input
    static void CreateQuestByUser()
    {
        Console.WriteLine("\n--- Create New Quest ---");
        Console.Write("Enter quest name: ");
        string name = Console.ReadLine() ?? "Unnamed Mission";
        Console.Write("Enter quest description: ");
        string description = Console.ReadLine() ?? "No description.";
        Console.Write("Enter quest priority (High/Medium/Low): ");
        string priority = Console.ReadLine() ?? "Medium";

        Quest newQuest = new Quest(name, description) { Priority = priority };
        AddQuest(newQuest);
        Console.WriteLine("\nQuest successfully created and saved to database!");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    // Add a quest to SQL
    static void AddQuest(Quest quest)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO dbo.Quests (Title, Description, Status, DueDate, Priority) VALUES (@title, @desc, @status, @due, @priority)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@title", quest.Name);
                command.Parameters.AddWithValue("@desc", quest.Description);
                command.Parameters.AddWithValue("@status", quest.Status.ToString());
                command.Parameters.AddWithValue("@due", quest.DueDate);
                command.Parameters.AddWithValue("@priority", quest.Priority);
                command.ExecuteNonQuery();
            }
        }
    }

    // Update a quest by ID
    static void UpdateQuest(int id, string newTitle, string newDescription, QuestStatus status, string priority)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE dbo.Quests SET Title = @title, Description = @desc, Status = @status, Priority = @priority WHERE Id = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", newTitle);
                command.Parameters.AddWithValue("@desc", newDescription);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Parameters.AddWithValue("@priority", priority);
                command.ExecuteNonQuery();
            }
        }
    }

    // Delete a quest by ID
    static void DeleteQuest(int id)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "DELETE FROM dbo.Quests WHERE Id = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }

    // Show all quests
    static void ShowAllQuests()
    {
        List<Quest> quests = new List<Quest>();
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
                    
                    quests.Add(q);
                }
            }
        }

        foreach (var quest in quests)
        {
            Console.WriteLine($"ID: {quest.ID} | Name: {quest.Name} | Priority: {quest.Priority} | Status: {quest.Status} | Deadline: {quest.DueDate.ToShortDateString()}");
        }
    }
}