using Microsoft.EntityFrameworkCore;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> products => Set<Product>();
    }
}
