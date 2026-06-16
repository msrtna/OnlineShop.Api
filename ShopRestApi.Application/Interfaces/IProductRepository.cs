using ShopRestApi.Application.Common.Models;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);

        Task<PagedResult<Product>> GetPagedAsync(PaginationParameters parameters);
    }
}