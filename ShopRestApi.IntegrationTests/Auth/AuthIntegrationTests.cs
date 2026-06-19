using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopRestApi.Application.DTOs.Auth;
using ShopRestApi.IntegrationTests.Infrastructure;

namespace ShopRestApi.IntegrationTests.Auth
{
    public class AuthIntegrationTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var request = new RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync(
                    "/api/auth/register",
                    request);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsToken()
        {
            // Arrange (اول Register)
            var register = new RegisterDto
            {
                Email = "login@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            await _client.PostAsJsonAsync(
                "/api/auth/register",
                register);

            var login = new LoginDto
            {
                Email = "login@test.com",
                Password = "Password123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var token =
                await response.Content
                    .ReadFromJsonAsync<AuthResponseDto>();

            token!.Token
                .Should()
                .NotBeNullOrEmpty();
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


    }
}
