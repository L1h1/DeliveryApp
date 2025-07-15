using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Models;

namespace UserService.DAL.Data
{
    public static class DbInitializer
    {
        // Use for test environment only
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles = { "Admin", "Client", "Courier" };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = new User()
            {
                Email = "initial@test.com",
                UserName = "initial",
            };
            await userManager.CreateAsync(user, "Password123!");
            var confToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, confToken);
        }
    }
}
