using BookMyMovies.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMyMovies.Repositories;
using BookMyMovies.Models;
using Microsoft.AspNetCore.Identity;
namespace BookMyMovies.Test
{
    public class MoviePostingRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public MoviePostingRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateDbContext()=> new ApplicationDbContext(_options);

        [Fact]
        public async Task AddAsync_ShouldAddMoviePosting()
        {
            var db = CreateDbContext();

            var repository = new MoviePostingRepository(db);

            var moviePosting = new MoviePosting
            {
                Title = "Test Title",
                ImageUrl = "/images/test.jpg",
                Description = "Test Description",
                Theater = "Test Theater",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "test-user-id",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 10,
                TotalSeats = 100
            };

            await repository.AddAsync(moviePosting);

            var result = db.MoviePostings.Find(moviePosting.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Title", result.Title);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnMoviePosting()
        {
            var db = CreateDbContext();
            var repository = new MoviePostingRepository(db);
            var moviePosting = new MoviePosting
            {
                Title = "Test Title",
                ImageUrl = "/images/test.jpg",
                Description = "Test Description",
                Theater = "Test Theater",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "test-user-id",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 10,
                TotalSeats = 100
            };

            await db.MoviePostings.AddAsync(moviePosting);
            await db.SaveChangesAsync();
            var result = await repository.GetByIdAsync(moviePosting.Id);

            Assert.NotNull(result);
            Assert.Equal(moviePosting.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMoviePostings()
        {
            var db = CreateDbContext();
            var repository = new MoviePostingRepository(db);
            var moviePosting1 = new MoviePosting
            {
                Title = "Test Title 1",
                ImageUrl = "/images/test1.jpg",
                Description = "Test Description 1",
                Theater = "Test Theater 1",
                Location = "Test Location 1",
                PostedDate = DateTime.Now,
                UserId = "test-user-id-1",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 10,
                TotalSeats = 100
            };
            var moviePosting2 = new MoviePosting
            {
                Title = "Test Title 2",
                ImageUrl = "/images/test2.jpg",
                Description = "Test Description 2",
                Theater = "Test Theater 2",
                Location = "Test Location 2",
                PostedDate = DateTime.Now,
                UserId = "test-user-id-2",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 20,
                TotalSeats = 200
            };

            await db.MoviePostings.AddRangeAsync(moviePosting1, moviePosting2);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateJobPosting()
        {
            // db context
            var db = CreateDbContext();

            // job posting repository
            var repository = new MoviePostingRepository(db);

            // job posting
            var moviePosting = new MoviePosting
            {
                Title = "Test Title",
                ImageUrl = "/images/test.jpg",
                Description = "Test Description",
                Theater = "Test Theater",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "test-user-id",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 10,
                TotalSeats = 100
            };
            await db.MoviePostings.AddAsync(moviePosting);
            await db.SaveChangesAsync();

            moviePosting.Description = "Updated Description";
            await repository.UpdateAsync(moviePosting);
            var result = db.MoviePostings.Find(moviePosting.Id);
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteMoviePosting()
        {
            var db = CreateDbContext();
            var repository = new MoviePostingRepository(db);
            var moviePosting = new MoviePosting
            {
                Title = "Test Title",
                ImageUrl = "/images/test.jpg",
                Description = "Test Description",
                Theater = "Test Theater",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "test-user-id",
                IsApproved = true,
                IsSeatAvailable = true,
                SeatsAvailable = 10,
                TotalSeats = 100
            };
            await db.MoviePostings.AddAsync(moviePosting);
            await db.SaveChangesAsync();

            await repository.DeleteAsync(moviePosting.Id);
            var result = db.MoviePostings.Find(moviePosting.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task History_ReturnViewResult_WithBookingForUser()
        {
            var db = CreateDbContext();

            var testUser = new IdentityUser
            {
                Id = "user123",
                UserName = "test@example.com"
            };

            var movie = new MoviePosting
            {
                Id = 1,
                Title = "Test Movie",
                Theater = "Test Theater",
                Location = "Test City",
                Description = "Test Desc",
                ImageUrl = "/test.jpg",
                IsApproved = true,
                SeatsAvailable = 10,
                TotalSeats = 10,
                UserId = "user123",
            };

            var booking = new Booking
            {
                Id = 1,
                UserId = "user123",
                MoviePostingId = 1,
                BookingDate = DateTime.UtcNow,
                PaymentStatus = "Paid",
                SeatNumbers = "A1,A2"
            };
            db.MoviePostings.Add(movie);
            db.Bookings.Add(booking);
            await db.SaveChangesAsync();

           
        }
    }
}
