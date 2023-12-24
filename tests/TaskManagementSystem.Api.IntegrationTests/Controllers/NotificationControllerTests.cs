using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagementSystem.Infrastructure.Identity;
using Xunit;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers;

public class NotificationControllerTests : IClassFixture<ApplicationFactory>
{
    private HttpClient _client;
    
    public NotificationControllerTests(ApplicationFactory applicationFactory)
    {
        _client = applicationFactory.CreateClient();
    }
    
    private async Task<string> GetTokenAsync()
    {
        var adminTokenRequest = new TokenRequest
        {
            Email = "admin@test.com",
            Password = "Adm1nPasswordd"
        };
        var adminResponse = await _client.PostAsJsonAsync("/api/auth/login", adminTokenRequest);
        return (await adminResponse.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
    }
    
    [Fact]
    public async Task GetAll_NoToken_ReturnsUnauthorized()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/users/1/notifications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetAll_WithToken_ReturnsListOfUsers()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users/1/notifications");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}