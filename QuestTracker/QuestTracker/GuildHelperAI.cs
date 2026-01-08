using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using dotenv.net; // Om du vill ladda .env-filen automatiskt

namespace HeroProject
{
    // Guild AI som hanterar konversationer med användaren
    public class GuildHelperAI
    {
        private static readonly HttpClient client = new HttpClient(); // Delad HttpClient för återanvändning
        private readonly string apiKey;

        // Konstruktor laddar miljövariabler och API-nyckel
        public GuildHelperAI()
        {
            // Ladda .env-filen
            DotEnv.Load();

            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OPENAI_API_KEY not found. Add it to your .env or environment variables.");
        }

        // Skickar användarens fråga till OpenAI och returnerar svaret
        public async Task<string> AskGuildAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return "Please enter a question.";

            var requestBody = new
            {
                model = "gpt-5-mini", // Ändra till nano/mini som du vill
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful Guild Advisor AI. End each answer with: 'Can I help you with something else?'"
                    },
                    new { role = "user", content = userMessage }
                }
            };

            string json = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseBody);
            string reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            return reply;
        }

        // Skriver ut svaret med färg (valfritt)
        public void PrintGuildMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green; // Du kan ändra färg
            Console.WriteLine($"Guild Advisor: {message}");
            Console.ResetColor();
        }

        // Enkel metod för interaktivt run-loop
        public async Task RunAsync()
        {
            Console.Clear();
            Console.WriteLine("Ask Guild Advisor something (type 'exit' to return):");

            while (true)
            {
                Console.Write("> ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                    continue;

                if (userInput.ToLower() == "exit")
                    break;

                string reply = await AskGuildAsync(userInput);
                PrintGuildMessage(reply);
                Console.WriteLine();
            }
        }

    }
}
