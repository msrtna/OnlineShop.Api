using Microsoft.AspNetCore.Identity;
using ShopRestApi.Application.Common.Constants;

namespace ShopRestApi.Infrastructure.Identity
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Customer))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(Roles.Customer));
            }
        }
    }
}
