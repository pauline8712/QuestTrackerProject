using dotenv.net;
using HeroProject;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

internal class Program
{
    // Connection string to your SQL Server database
    static string connectionString = @"Server=.\SQLEXPRESS;Database=QuestDB;Trusted_Connection=True;";

    private static void Main(string[] args)
    {
        DotEnv.Load(); // Load .env variables
        Console.Title = "Quest Guild Tracker";
        Console.ForegroundColor = ConsoleColor.Green;

        // Example usage of SQL quests
        AddQuest(new Quest("Rescue the Princess", "Go to the castle and rescue the princess."));
        AddQuest(new Quest("Find the Treasure", "Explore the cave and find the treasure."));

        Console.WriteLine("All quests after adding:");
        ShowAllQuests();

        UpdateQuest(1, "Rescue the Prince", "Go to the castle and rescue the prince.");
        Console.WriteLine("\nAll quests after updating ID 1:");
        ShowAllQuests();

        DeleteQuest(2);
        Console.WriteLine("\nAll quests after deleting ID 2:");
        ShowAllQuests();

        // Start main menu from MenuHelper
        MenuHelper.StartMenu();

        Console.ResetColor();
    }

    // Quest class
    class Quest
    {
        public int Id { get; set; } // Auto-incremented by SQL
        public string Name { get; set; }
        public string Description { get; set; }

        public Quest(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    // Add a quest to SQL
    static void AddQuest(Quest quest)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Quests (Name, Description) VALUES (@name, @desc)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", quest.Name);
                command.Parameters.AddWithValue("@desc", quest.Description);
                command.ExecuteNonQuery();
            }
        }
    }

    // Update a quest by ID
    static void UpdateQuest(int id, string newName, string newDescription)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE Quests SET Name = @name, Description = @desc WHERE Id = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@desc", newDescription);
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
            string query = "DELETE FROM Quests WHERE Id = @id";
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
            string query = "SELECT Id, Name, Description FROM Quests";
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    quests.Add(new Quest(reader["Name"].ToString(), reader["Description"].ToString())
                    {
                        Id = (int)reader["Id"]
                    });
                }
            }
        }

        foreach (var quest in quests)
        {
            Console.WriteLine($"ID: {quest.Id} | Name: {quest.Name} | Description: {quest.Description}");
        }
    }
}