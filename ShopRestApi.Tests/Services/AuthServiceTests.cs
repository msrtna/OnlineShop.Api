using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using ShopRestApi.Application.Common.Settings;
using ShopRestApi.Application.DTOs.Auth;
using ShopRestApi.Infrastructure.Identity;
using ShopRestApi.Infrastructure.Services;
using ShopRestApi.Tests.Helpers;

namespace ShopRestApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>>
            _userManagerMock;

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userManagerMock =
                UserManagerMockHelper
                    .MockUserManager<ApplicationUser>();

            var jwtSettings =
                Options.Create(
                    new JwtSettings
                    {
                        Key =
                            "ThisIsMySuperSecretKeyForJwtToken12345",

                        Issuer =
                            "ShopRestApi",

                        Audience =
                            "ShopRestApiUsers",

                        DurationInMinutes = 60
                    });

            _service =
                new AuthService(
                    _userManagerMock.Object,
                    jwtSettings);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange

            var user = new ApplicationUser
            {
                Id = "1",
                Email = "test@test.com",
                UserName = "test@test.com"
            };

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        "test@test.com"))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x =>
                    x.CheckPasswordAsync(
                        user,
                        "Password123!"))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x =>
                    x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string> { "Customer" });

            // Act

            var result =
                await _service.LoginAsync(
                    new LoginDto
                    {
                        Email = "test@test.com",
                        Password = "Password123!"
                    });

            // Assert

            Assert.NotNull(result);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    result.AccessToken));

            Assert.False(
                string.IsNullOrWhiteSpace(
                    result.RefreshToken));
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsException()
        {
            // Arrange

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        It.IsAny<string>()))
                .ReturnsAsync(
                    (ApplicationUser?)null);

            // Act + Assert

            await Assert.ThrowsAsync<Exception>(
                () =>
                    _service.LoginAsync(
                        new LoginDto
                        {
                            Email = "abc@test.com",
                            Password = "123"
                        }));
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsException()
        {
            // Arrange

            var user = new ApplicationUser
            {
                Email = "test@test.com"
            };

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x =>
                    x.CheckPasswordAsync(
                        user,
                        It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act + Assert

            await Assert.ThrowsAsync<Exception>(
                () =>
                    _service.LoginAsync(
                        new LoginDto
                        {
                            Email = "test@test.com",
                            Password = "WrongPassword"
                        }));
        }

        [Fact]
        public async Task RegisterAsync_NewUser_CreatesUser()
        {
            // Arrange

            _userManagerMock
                .Setup(x =>
                    x.FindByEmailAsync(
                        It.IsAny<string>()))
                .ReturnsAsync(
                    (ApplicationUser?)null);

            _userManagerMock
                .Setup(x =>
                    x.CreateAsync(
                        It.IsAny<ApplicationUser>(),
                        It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Success);

            _userManagerMock
                .Setup(x =>
                    x.AddToRoleAsync(
                        It.IsAny<ApplicationUser>(),
                        It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Success);

            // Act

            await _service.RegisterAsync(
                new RegisterDto
                {
                    Email = "new@test.com",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                });

            // Assert

            _userManagerMock.Verify(
                x =>
                    x.CreateAsync(
                        It.IsAny<ApplicationUser>(),
                        It.IsAny<string>()),
                Times.Once);

            _userManagerMock.Verify(
                x =>
                    x.AddToRoleAsync(
                        It.IsAny<ApplicationUser>(),
                        "Customer"),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            // Arrange

            var user = new ApplicationUser
            {
                Id = "1",
                Email = "test@test.com",

                RefreshToken = "OLD_TOKEN",

                RefreshTokenExpiryTime =
                    DateTime.UtcNow.AddDays(1)
            };

            var users =
                new List<ApplicationUser>
                {
            user
                }.AsQueryable();

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(users);

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string>
                    {
                "Customer"
                    });

            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(
                    IdentityResult.Success);

            // Act

            var result =
                await _service.RefreshTokenAsync(
                    new RefreshTokenRequestDto
                    {
                        RefreshToken =
                            "OLD_TOKEN"
                    });

            // Assert

            Assert.NotNull(result);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    result.AccessToken));

            Assert.False(
                string.IsNullOrWhiteSpace(
                    result.RefreshToken));

            _userManagerMock.Verify(
                x => x.UpdateAsync(user),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ThrowsException()
        {
            // Arrange

            var users =
                new List<ApplicationUser>()
                .AsQueryable();

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(users);

            // Act + Assert

            await Assert.ThrowsAsync<Exception>(
                () =>
                    _service.RefreshTokenAsync(
                        new RefreshTokenRequestDto
                        {
                            RefreshToken =
                                "INVALID_TOKEN"
                        }));
        }

        [Fact]
        public async Task RefreshTokenAsync_ExpiredToken_ThrowsException()
        {
            // Arrange

            var user =
                new ApplicationUser
                {
                    RefreshToken =
                        "TOKEN",

                    RefreshTokenExpiryTime =
                        DateTime.UtcNow.AddDays(-1)
                };

            var users =
                new List<ApplicationUser>
                {
            user
                }.AsQueryable();

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(users);

            // Act + Assert

            await Assert.ThrowsAsync<Exception>(
                () =>
                    _service.RefreshTokenAsync(
                        new RefreshTokenRequestDto
                        {
                            RefreshToken =
                                "TOKEN"
                        }));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldRotateRefreshToken()
        {
            // Arrange

            var user = new ApplicationUser
            {
                Id = "1",
                Email = "test@test.com",

                RefreshToken = "OLD_TOKEN",

                RefreshTokenExpiryTime =
                    DateTime.UtcNow.AddDays(1)
            };

            var oldToken =
                user.RefreshToken;

            var users =
                new List<ApplicationUser>
                {
            user
                }.AsQueryable();

            _userManagerMock
                .Setup(x => x.Users)
                .Returns(users);

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(
                    new List<string>
                    {
                "Customer"
                    });

            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(
                    IdentityResult.Success);

            // Act

            await _service.RefreshTokenAsync(
                new RefreshTokenRequestDto
                {
                    RefreshToken =
                        "OLD_TOKEN"
                });

            // Assert

            Assert.NotEqual(
                oldToken,
                user.RefreshToken);
        }
    }
}
