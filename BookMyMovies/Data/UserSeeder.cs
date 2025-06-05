using BookMyMovies.Constants;
using Microsoft.AspNetCore.Identity;

namespace BookMyMovies.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager, "Admin@gmail.com", "A2022@abc", Roles.Admin);

            await CreateUserWithRole(userManager, "Employer@gmail.com", "E2022@abc", Roles.Employer);

            await CreateUserWithRole(userManager, "User@gmail.com", "U2022@abc", Roles.User);
        }

        private static async Task CreateUserWithRole(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
         if(await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Failed creating user with email {user.Email}: Errors: {string.Join(",", result.Errors)}");
                }
            }
        }
    }
}
