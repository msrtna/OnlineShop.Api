using Microsoft.EntityFrameworkCore;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;
using ShopRestApi.Infrastructure.Persistence;

namespace ShopRestApi.Application.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.products.ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.products.FirstOrDefaultAsync(p=> p.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
    }
}
