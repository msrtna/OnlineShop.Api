using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Domain.Entities;
using ShopRestApi.Infrastructure.Services;

namespace ShopRestApi.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();

            _mapperMock = new Mock<IMapper>();

            _service = new ProductService(_repositoryMock.Object, _mapperMock.Object);
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

        [Fact]
        public async Task AddAsync_ShouldSetCreatedAt()
        {
            // Arrange

            var product = new Product
            {
                Name = "Keyboard"
            };

            // Act

            await _service.AddAsync(product);

            // Assert

            Assert.NotEqual(
                default(DateTime),
                product.CreatedAt);

            _repositoryMock.Verify(
                x => x.AddAsync(product),
                Times.Once);
        }


    }
}
