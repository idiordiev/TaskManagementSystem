using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagementSystem.Api.IntegrationTests.Controllers.Fixtures;
using TaskManagementSystem.Application.Users.Commands;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Infrastructure.Identity;
using Xunit;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers;

[TestCaseOrderer("TaskManagementSystem.Api.IntegrationTests.PriorityOrderer", "TaskManagementSystem.Api.IntegrationTests")]
public class UserControllerTests : IClassFixture<ApplicationFactory>, IClassFixture<UserControllerStateFixture>
{
    private HttpClient _client;
    private UserControllerStateFixture _state;
    
    public UserControllerTests(ApplicationFactory applicationFactory, UserControllerStateFixture state)
    {
        _state = state;
        _client = applicationFactory.CreateClient();
    }
    
    private async Task<string> GetAdminTokenAsync()
    {
        var tokenRequest = new TokenRequest
        {
            Email = "admin@test.com",
            Password = "Adm1nPasswordd"
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", tokenRequest);
        return (await response.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
    }
    
    private async Task<string> GetUserTokenAsync()
    {
        var tokenRequest = new TokenRequest
        {
            Email = _state.CreateUserCommand.Email,
            Password = _state.CreateUserCommand.Password
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", tokenRequest);
        return (await response.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_NoToken_ReturnsUnauthorized()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_UserToken_ReturnsForbidden()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetUserTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_AdminToken_ReturnsListOfUsers()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var users = await response.Content.ReadFromJsonAsync<UserResponse[]>();
        users.Should().NotBeEmpty();
    }
    
    [Fact, TestPriority(3)]
    public async Task GetById_AdminId_ReturnsAdmin()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Email.Should().Be("admin@test.com");
    }
    
    [Fact, TestPriority(3)]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users/100000124");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, TestPriority(1)]
    public async Task Create_NewUser_ReturnsCreatedAndUserResponse()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        var createUserContract = _state.CreateUserCommand;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().NotBe(0);
        user.Email.Should().Be(createUserContract.Email);
        user.Name.Should().Be(createUserContract.Name);

        _state.UserId = user.Id;
    }
    
    [Fact, TestPriority(1)]
    public async Task Create_ExistingUser_ReturnsBadRequest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        var createUserContract = new CreateUserCommand
        {
            Name = "admin",
            Email = "admin@test.com",
            Password = "Adm1nPasswordd",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact, TestPriority(4)]
    public async Task Update_ExistingUser_ReturnsOk()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        var updateUserContract = new UpdateUserCommand
        {
            Name = "admin new name"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1", updateUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Email.Should().Be("admin@test.com");
        user.Name.Should().Be("admin new name");
    }
    
    [Fact, TestPriority(4)]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        var updateUserContract = new UpdateUserCommand
        {
            Name = "not existing user new name"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/users/100000124", updateUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, TestPriority(5)]
    public async Task Delete_ExistingUser_ReturnsOk()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        
        // Act
        var response = await _client.DeleteAsync($"/api/users/{_state.UserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact, TestPriority(5)]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync());
        
        // Act
        var response = await _client.DeleteAsync("/api/users/100000124");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}