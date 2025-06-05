using BookMyMovies.Data;
using BookMyMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMyMovies.Repositories
{
    public class MoviePostingRepository : IRepository<MoviePosting>
    {
        private readonly ApplicationDbContext _context;
        public MoviePostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MoviePosting entity)
        {
            await _context.MoviePostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var moviePosting = await _context.MoviePostings.FindAsync(id);
            if (moviePosting == null)
            {
                throw new KeyNotFoundException($"Job Posting with ID {id} not found.");
            }
            _context.MoviePostings.Remove(moviePosting);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MoviePosting>> GetAllAsync()
        {
            return await _context.MoviePostings.ToListAsync();
        }

        public async Task<MoviePosting> GetByIdAsync(int id)
        {
            var moviePosting = await _context.MoviePostings.FindAsync(id);
            if (moviePosting == null)
            {
                throw new KeyNotFoundException($"MoviePosting with ID {id} not found.");
            }
            return moviePosting;
        }

        public async Task UpdateAsync(MoviePosting entity)
        {
             _context.MoviePostings.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
