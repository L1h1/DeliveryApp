using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.DAL.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles = { "Admin", "Client", "Courier" };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
