using BookMyMovies.Data;
using BookMyMovies.Models;
using BookMyMovies.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookMyMovies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
                }
             ); 

            builder.Services.AddDefaultIdentity<IdentityUser>(
            options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                  
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<IRepository<MoviePosting>,
                MoviePostingRepository>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                RoleSeeder.SeedRolesAsync(services).Wait();
                UserSeeder.SeedUsersAsync(services).Wait();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages(); // For Identity pages

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=MoviePostings}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
