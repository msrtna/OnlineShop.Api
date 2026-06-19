using AutoMapper;
using Castle.Core.Logging;
using Moq;
using ShopRestApi.Application.Common.Models;
using ShopRestApi.Application.DTOs.ProductsDtos;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;
using ShopRestApi.Infrastructure.Services;

namespace ShopRestApi.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();

            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger>();

            _service = new ProductService(_repositoryMock.Object, _mapperMock.Object, (Microsoft.Extensions.Logging.ILogger)_loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsProduct()
        {
            // Arrange

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

            // Act

            var result = await _service.GetByIdAsync(1);

            // Assert

            Assert.NotNull(result);

            Assert.Equal(1, result.Id);

            Assert.Equal("Laptop", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ProductNotFound_ReturnsNull()
        {
            // Arrange

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act

            var result = await _service.GetByIdAsync(1);

            // Assert

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ProductExists_ReturnsTrue()
        {
            // Arrange

            var product = new Product
            {
                Id = 1,
                Name = "Phone"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act

            var result =
                await _service.DeleteAsync(1);

            // Assert

            Assert.True(result);

            _repositoryMock.Verify(
                x => x.DeleteAsync(product),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ProductNotFound_ReturnsFalse()
        {
            // Arrange

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Product?)null);

            // Act

            var result =
                await _service.DeleteAsync(1);

            // Assert

            Assert.False(result);

            _repositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Product>()),
                Times.Never);
        }

        //[Fact]
        //public async Task AddAsync_ShouldSetCreatedAt()
        //{
        //    // Arrange

        //    var product = new CreateProductDto
        //    {
        //        Name = "Keyboard"
        //    };

        //    // Act

        //    await _service.AddAsync(product);

        //    // Assert

        //    Assert.NotEqual(
        //        default(DateTime),
        //        product.CreatedAt);

        //    _repositoryMock.Verify(
        //        x => x.AddAsync(product),
        //        Times.Once);
        //}

        [Fact]
        public async Task UpdateAsync_ProductExists_ReturnsTrue()
        {
            // Arrange

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

            // Act

            var result = await _service.UpdateAsync(1, dto);

            // Assert

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
            // Arrange

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

            // Act

            var result = await _service.UpdateAsync(1, dto);

            // Assert

            Assert.False(result);

            _repositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Product>()),
                Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsProducts()
        {
            // Arrange

            var products = new List<Product>
    {
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Phone" }
    };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(products);

            // Act

            var result = await _service.GetAllAsync();

            // Assert

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            // Arrange

            var products = new List<Product>
    {
        new Product
        {
            Id = 1,
            Name = "Laptop"
        }
    };

            var productDtos = new List<ProductDto>
    {
        new ProductDto
        {
            Id = 1,
            Name = "Laptop"
        }
    };

            var pagedResult =
                new PagedResult<Product>
                {
                    Items = products,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalCount = 1
                };

            var parameters =
                new ProductQueryParameters();

            _repositoryMock
                .Setup(x => x.GetPagedAsync(parameters))
                .ReturnsAsync(pagedResult);

            _mapperMock
                .Setup(x => x.Map<List<ProductDto>>(products))
                .Returns(productDtos);

            // Act

            var result =
                await _service.GetPagedAsync(parameters);

            // Assert

            Assert.Single(result.Items);

            Assert.Equal(1, result.TotalCount);

            Assert.Equal("Laptop",
                result.Items.First().Name);
        }
    }
}
