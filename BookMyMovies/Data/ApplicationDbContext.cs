using BookMyMovies.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookMyMovies.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<MoviePosting> MoviePostings { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.MoviePosting)
                .WithMany()
                .HasForeignKey(b => b.MoviePostingId)
                .OnDelete(DeleteBehavior.Restrict);  // <- restrict to avoid cascade

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // <- restrict this too
        }

    }
}
