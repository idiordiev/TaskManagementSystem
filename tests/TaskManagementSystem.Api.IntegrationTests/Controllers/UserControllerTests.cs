using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using Xunit;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<ApplicationFactory>
{
    private HttpClient _client;
    private string _jwtToken;
    private string _regularUserToken;
    private int _userId;
    
    public UserControllerTests(ApplicationFactory applicationFactory)
    {
        _client = applicationFactory.CreateClient();
        
        OneTimeSetUp();
    }
    
    public void OneTimeSetUp()
    {
        var newUserContract = new CreateUserContract
        {
            Name = Guid.NewGuid().ToString(),
            Email = $"{Guid.NewGuid().ToString()}@mail.com",
            Password = Guid.NewGuid().ToString()
        };

        var newUserResponse = _client.PostAsJsonAsync("/api/auth/register", newUserContract).GetAwaiter().GetResult();
        _userId = newUserResponse.Content.ReadFromJsonAsync<UserResponse>().GetAwaiter().GetResult()!.Id;
        
        var adminTokenRequest = new TokenRequest
        {
            Email = "admin@test.com",
            Password = "Adm1nPasswordd"
        };
        var adminResponse = _client.PostAsJsonAsync("/api/auth/login", adminTokenRequest).GetAwaiter().GetResult();
        _jwtToken = adminResponse.Content.ReadFromJsonAsync<TokenResponse>().GetAwaiter().GetResult()!.AccessToken;
        
        var userTokenRequest = new TokenRequest
        {
            Email = newUserContract.Email,
            Password = newUserContract.Password
        };
        var userResponse =  _client.PostAsJsonAsync("/api/auth/login", userTokenRequest).GetAwaiter().GetResult();
        _regularUserToken = userResponse.Content.ReadFromJsonAsync<TokenResponse>().GetAwaiter().GetResult()!.AccessToken;

    }
    
    [Fact]
    public async Task GetAll_NoToken_ReturnsUnauthorized()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetAll_UserToken_ReturnsForbidden()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _regularUserToken);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task GetAll_AdminToken_ReturnsListOfUsers()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var users = await response.Content.ReadFromJsonAsync<UserResponse[]>();
        users.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task GetById_AdminId_ReturnsAdmin()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Email.Should().Be("admin@test.com");
    }
    
    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

        // Act
        var response = await _client.GetAsync("/api/users/100000124");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Create_NewUser_ReturnsCreatedAndUserResponse()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        var createUserContract = new CreateUserContract
        {
            Name = "newusersfdkj",
            Email = "userjahsfj@test.com",
            Password = "somE43paas"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Id.Should().NotBe(0);
        user.Email.Should().Be(createUserContract.Email);
        user.Name.Should().Be(createUserContract.Name);
    }
    
    [Fact]
    public async Task Create_ExistingUser_ReturnsBadRequest()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        var createUserContract = new CreateUserContract
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
    
    [Fact]
    public async Task Update_ExistingUser_ReturnsOk()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        var updateUserContract = new UpdateUserContract
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
    
    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        var updateUserContract = new UpdateUserContract
        {
            Name = "not existing user new name"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/users/100000124", updateUserContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_ExistingUser_ReturnsOk()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        
        // Act
        var response = await _client.DeleteAsync($"/api/users/{_userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        
        // Act
        var response = await _client.DeleteAsync("/api/users/100000124");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}