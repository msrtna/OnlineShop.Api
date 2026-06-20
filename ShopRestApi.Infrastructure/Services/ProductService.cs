using AutoMapper;
using Microsoft.Extensions.Logging;
using ShopRestApi.Api.Exceptions;
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
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository repository, IMapper mapper, ILogger<ProductService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<List<ProductDto>>(result);
        }
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found: {ProductId}",
                    id);

                throw new NotFoundException(
                    $"Product {id} not found");
            }
            return _mapper.Map<ProductDto>(product);
        }
        public async Task<ProductDto> AddAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            if (dto.Price <= 0)
            {
                throw new ValidationException(
                    "Price must be greater than zero");
            }
            product.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(product);
            _logger.LogInformation("Creating product {ProductName}", dto.Name);

            return _mapper.Map<ProductDto>(product);
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null)
                return false;
            if (dto.Price <= 0)
            {
                throw new ValidationException(
                    "Price must be greater than zero");
            }
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;

            await _repository.UpdateAsync(product);
            _logger.LogInformation("Updating product with Id: {ProductId}", id);
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
                return false;

            await _repository.DeleteAsync(product);

            _logger.LogWarning("Deleting product with Id: {ProductId}", id);

            return true;
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductQueryParameters parameters)
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
