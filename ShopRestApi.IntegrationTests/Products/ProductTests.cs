using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ShopRestApi.Application.DTOs.Auth;
using ShopRestApi.IntegrationTests.Infrastructure;

namespace ShopRestApi.IntegrationTests.Products
{
    public class ProductTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response =
                await _client.GetAsync("/api/products");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetProducts_WithToken_ReturnsOk()
        {
            // Arrange (Register + Login)
            var email = "auth@test.com";

            await _client.PostAsJsonAsync("/api/auth/register",
                new RegisterDto
                {
                    Email = email,
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                });

            var loginResponse =
                await _client.PostAsJsonAsync("/api/auth/login",
                new LoginDto
                {
                    Email = email,
                    Password = "Password123!"
                });

            var auth =
                await loginResponse.Content
                    .ReadFromJsonAsync<AuthResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    auth!.Token);

            // Act
            var response =
                await _client.GetAsync("/api/products");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Ping_ReturnsOk()
        {
            var response = await _client.GetAsync("/ping");

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }
    }
}
