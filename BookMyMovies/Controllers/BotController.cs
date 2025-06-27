using BookMyMovies.Data;
using BookMyMovies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BookMyMovies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _gemini;
        public BotController(ApplicationDbContext context,GeminiService gemini)
        {
            _context = context;
            _gemini = gemini;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            string message = request?.Message?.ToLower().Trim();
            string reply;

            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Empty message.");
            }



            else if (message.Contains("hi") || message.Contains("hello") || message.Contains("hey"))
            {
                reply = "👋 Hello! I'm your Movie Assistant. Ask me about bookings, seat availability, or say 'help' for options.";
            }
            else if(message.Contains("book"))
            {
                reply = "🎟️ To book a movie, go to the Movie Details page and click 'Book Now'.";
            }
            else if (message.Contains("seat"))
            {
                var allMovies = await _context.MoviePostings.ToListAsync();
                var matchedMovie = allMovies.FirstOrDefault(m => message.Contains(m.Title.ToLower()));

                if (matchedMovie != null)
                {
                    reply = $"🎬 '{matchedMovie.Title}' has {matchedMovie.SeatsAvailable} out of {matchedMovie.TotalSeats} seats available.";
                }
                else
                {
                    // Default to most recent if no match
                    var mostRecent = allMovies.OrderByDescending(m => m.PostedDate).FirstOrDefault();
                    if (mostRecent != null)
                    {
                        reply = $"ℹ️ Couldn't detect a specific movie name, but '{mostRecent.Title}' has {mostRecent.SeatsAvailable} out of {mostRecent.TotalSeats} seats.";
                    }
                    else
                    {
                        reply = "⚠️ No movies found currently.";
                    }
                }
            }
            else if (message.Contains("help"))
            {
                reply = "🤖 You can ask me about booking, seat availability, or movie timings.";
            }
            else
            {
                reply = await _gemini.AskGeminiAsync($"You are a chatbot for a movie ticket booking website. The user asked: \"{request.Message}\". Reply helpfully related to movie booking.");
            }

            return Ok(new { reply });
        }
    }
    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
