

namespace HeroProject
{
    using HeroProject;
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;


    public class GuildHelperAI
    {
        //har inget objekt...
        private readonly HttpClient client;
        private readonly string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");


        public GuildHelperAI()
        {
            client = new HttpClient();
        }


        public async Task RunAsync()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found.");
                return;
            }


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);


            Console.WriteLine("Ask Guild Advisor something:");
            string userInput = Console.ReadLine();


            var requestBody = new
            {
                model = "gpt-5-mini",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant. End each answer with can I help you with something else?" },
                new { role = "user", content = userInput }
            }
            };


            string json = JsonSerializer.Serialize(requestBody);


            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            string responseString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseString);


            string reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;


            Console.WriteLine("\nGuild Advisor: " + reply);
        }
    }
}
