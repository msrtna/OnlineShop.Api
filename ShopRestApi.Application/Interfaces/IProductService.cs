using ShopRestApi.Application.Common.Models;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task<bool> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);

        Task<PagedResult<ProductDto>> GetPagedAsync(PaginationParameters parameters);
    }
}