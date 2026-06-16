using Microsoft.EntityFrameworkCore;
using ShopRestApi.Application.Common.Models;
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
            return await _context.Products.ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p=> p.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Product>> GetPagedAsync(PaginationParameters parameters)
        {
            var totalCount = await _context.Products.CountAsync();

            var items = await _context.Products
                .OrderBy(p => p.Id)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
