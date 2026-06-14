using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task AddAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(product);
        }
    }
}
