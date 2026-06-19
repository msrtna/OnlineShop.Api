using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopRestApi.Api;
using ShopRestApi.Application.Common.Constants;
using ShopRestApi.Infrastructure.Identity;
using ShopRestApi.Infrastructure.Persistence;

namespace ShopRestApi.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(async services =>
        {
            var descriptor =
                services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    Guid.NewGuid().ToString());
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            var roleManager =
    scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(
                new IdentityRole(Roles.Admin));

            await roleManager.CreateAsync(
                new IdentityRole(Roles.Customer));
        });
    }
}