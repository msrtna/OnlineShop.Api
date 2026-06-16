using AutoMapper;
using ShopRestApi.Application.Common.Models;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;

namespace ShopRestApi.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null)
                return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;

            await _repository.UpdateAsync(product);

            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
                return false;

            await _repository.DeleteAsync(product);

            return true;
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(PaginationParameters parameters)
        {
            var result = await _repository.GetPagedAsync(parameters);

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<List<ProductDto>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
