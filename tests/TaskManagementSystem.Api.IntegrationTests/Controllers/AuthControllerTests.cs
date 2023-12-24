using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Infrastructure.Identity;
using Xunit;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<ApplicationFactory>
{
    private HttpClient _client;
    
    public AuthControllerTests(ApplicationFactory applicationFactory)
    {
        _client = applicationFactory.CreateClient();
    }

    [Fact]
    public async Task Login_UserExists_ReturnsToken()
    {
        // Arrange
        var tokenRequest = new TokenRequest
        {
            Email = "admin@test.com",
            Password = "Adm1nPasswordd"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", tokenRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsUnauthorized()
    {
        // Arrange
        var tokenRequest = new TokenRequest
        {
            Email = "admin123124@test.com",
            Password = "Adm1nPasswordd"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", tokenRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Register_UserExists_ReturnsBadRequest()
    {
        // Arrange
        var createUserContract = new CreateUserContract
        {
            Name = "admin",
            Email = "admin@test.com",
            Password = "Adm1nPasswordd",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", createUserContract);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }
    
    [Fact]
    public async Task Register_UserDoesNotExist_CreatesUser()
    {
        // Arrange
        var createUserContract = new CreateUserContract
        {
            Name = "admin2",
            Email = "admin123124@test.com",
            Password = "Adm1nPasswordd"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", createUserContract);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var userResponse = await JsonSerializer.DeserializeAsync<UserResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        userResponse.Should().NotBeNull();
        userResponse.Id.Should().NotBe(0);
        userResponse.Email.Should().Be(createUserContract.Email);
    }
}