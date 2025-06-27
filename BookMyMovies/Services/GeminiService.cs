using RestSharp;
using System.Text.Json;

namespace BookMyMovies.Services
{
    public class GeminiService
    {
        private readonly string _apiKey = "AIzaSyDHL-JtFdDLU7zQHGOBPh5bhEAMTPwmnd8";
        private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public async Task<string> AskGeminiAsync(string prompt)
        {
            var client = new RestClient($"{_endpoint}?key={_apiKey}");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                contents = new[]
                {
                new {
                    parts = new[] { new { text = prompt } }
                }
            }
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful) return "⚠️ Sorry, I'm having trouble right now.";

            using var doc = JsonDocument.Parse(response.Content);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }
    }
}
