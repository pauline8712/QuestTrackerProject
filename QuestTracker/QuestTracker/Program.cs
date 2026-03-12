using dotenv.net;
using HeroProject;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

internal class Program
{


    private static void Main(string[] args)
    {
        DotEnv.Load(); // Laddar alla variabler från .env
        Console.Title = "Quest Guild Tracker";
        Console.ForegroundColor = ConsoleColor.Green;

        // Startar huvudmenyn från MenuHelper-klassen
        MenuHelper.StartMenu();

        Console.ResetColor();
    }

}