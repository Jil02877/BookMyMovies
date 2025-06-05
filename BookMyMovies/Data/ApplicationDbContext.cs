using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookMyMovies.Models;

namespace BookMyMovies.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<MoviePosting> MoviePostings { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

        {
            
        }
    }
}
