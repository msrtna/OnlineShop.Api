using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ShopRestApi.Api.Exceptions;
using ShopRestApi.Application.Common.Models;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;
using ShopRestApi.Infrastructure.Services;

namespace ShopRestApi.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _service = new ProductService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsProductDto()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Laptop"
        };

        var dto = new ProductDto
        {
            Id = 1,
            Name = "Laptop"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ThrowsNotFoundException()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedProducts()
    {
        var products = new List<Product>
    {
        new() { Id = 1, Name = "Laptop" },
        new() { Id = 2, Name = "Phone" }
    };

        var productDtos = new List<ProductDto>
    {
        new() { Id = 1, Name = "Laptop" },
        new() { Id = 2, Name = "Phone" }
    };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(products))
            .Returns(productDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AddAsync_ValidProduct_ReturnsProductDto()
    {
        var dto = new CreateProductDto
        {
            Name = "Keyboard",
            Price = 100
        };

        var entity = new Product();

        var productDto = new ProductDto
        {
            Name = "Keyboard"
        };

        _mapperMock
            .Setup(x => x.Map<Product>(dto))
            .Returns(entity);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(entity))
            .Returns(productDto);

        var result = await _service.AddAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Keyboard", result.Name);

        Assert.NotEqual(default, entity.CreatedAt);

        _repositoryMock.Verify(
            x => x.AddAsync(entity),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_InvalidPrice_ThrowsValidationException()
    {
        var dto = new CreateProductDto
        {
            Name = "Keyboard",
            Price = 0
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.AddAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_ProductExists_ReturnsTrue()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Desc",
            Price = 100,
            StockQuantity = 5
        };

        var dto = new UpdateProductDto
        {
            Name = "New Name",
            Description = "New Desc",
            Price = 200,
            StockQuantity = 10
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result = await _service.UpdateAsync(1, dto);

        Assert.True(result);

        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(200, product.Price);
        Assert.Equal(10, product.StockQuantity);

        _repositoryMock.Verify(
            x => x.UpdateAsync(product),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ReturnsFalse()
    {
        var dto = new UpdateProductDto
        {
            Name = "Test",
            Description = "Test",
            Price = 100,
            StockQuantity = 1
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        var result = await _service.UpdateAsync(1, dto);

        Assert.False(result);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_InvalidPrice_ThrowsValidationException()
    {
        var product = new Product
        {
            Id = 1
        };

        var dto = new UpdateProductDto
        {
            Price = 0
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.UpdateAsync(1, dto));
    }

    [Fact]
    public async Task DeleteAsync_ProductExists_ReturnsTrue()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Phone"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);

        _repositoryMock.Verify(
            x => x.DeleteAsync(product),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ReturnsFalse()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);

        _repositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsMappedPagedResult()
    {
        var products = new List<Product>
    {
        new()
        {
            Id = 1,
            Name = "Laptop"
        }
    };

        var productDtos = new List<ProductDto>
    {
        new()
        {
            Id = 1,
            Name = "Laptop"
        }
    };

        var pagedResult = new PagedResult<Product>
        {
            Items = products,
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };

        var parameters = new ProductQueryParameters();

        _repositoryMock
            .Setup(x => x.GetPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(products))
            .Returns(productDtos);

        var result = await _service.GetPagedAsync(parameters);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Laptop", result.Items.First().Name);
    }

}