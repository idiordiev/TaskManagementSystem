using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.Api.IntegrationTests.Controllers.Fixtures;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.DAL.Enums;
using Xunit;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers;

[TestCaseOrderer("TaskManagementSystem.Api.IntegrationTests.PriorityOrderer", "TaskManagementSystem.Api.IntegrationTests")]
public class TaskControllerTests : IClassFixture<ApplicationFactory>, IClassFixture<TaskControllerStateFixture>
{
    private HttpClient _client;
    
    private TaskControllerStateFixture _state;
    
    public TaskControllerTests(ApplicationFactory applicationFactory, TaskControllerStateFixture state)
    {
        _state = state;
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
    
    [Fact, TestPriority(1)]
    public async Task Create_NewTask_AddsAndReturnsTaskResponse()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
        var createTaskContract = new CreateTaskContract
        {
            Name = "New task",
            Category = "somecat",
            DeadLine = null,
            Subtasks = new List<CreateSubtaskContract>
            {
                new CreateSubtaskContract { Name = "Subtask1" },
                new CreateSubtaskContract { Name = "Subtask2" },
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/1/tasks", createTaskContract);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task.Should().NotBeNull();
        task.Id.Should().NotBe(0);
        task.DeadLine.Should().Be(createTaskContract.DeadLine);
        task.Name.Should().Be(createTaskContract.Name);
        task.Category.Should().Be(createTaskContract.Category);
        task.UserId.Should().Be(1);

        _state.TaskId = task.Id;
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_NoToken_ReturnsUnauthorized()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/users/1/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_TaskExists_ReturnsListOfTasks()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users/1/tasks");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var tasks = await response.Content.ReadFromJsonAsync<TaskResponse[]>();
        tasks.Should().NotBeEmpty();
    }
    
    [Fact, TestPriority(3)]
    public async Task GetById_TaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

        // Act
        var response = await _client.GetAsync("/api/users/1/tasks/1341263871");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, TestPriority(3)]
    public async Task GetById_ExistingTask_ReturnsTaskResponse()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

        // Act
        var response = await _client.GetAsync($"/api/users/1/tasks/{_state.TaskId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task.Should().NotBeNull();
        task.Id.Should().Be(_state.TaskId);
    }
    
    [Fact, TestPriority(4)]
    public async Task Update_TaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
        var updateTaskContract = new UpdateTaskContract()
        {
            Name = "NewName for task",
            DeadLine = new DateTime(2023, 12, 24, 12, 45, 23, DateTimeKind.Utc),
            State = TaskState.InProgress
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1/tasks/1341263871", updateTaskContract);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, TestPriority(4)]
    public async Task Update_ExistingTask_UpdatesAndReturnsTaskResponse()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
        var updateTaskContract = new UpdateTaskContract()
        {
            Name = "NewName for task",
            DeadLine = new DateTime(2023, 12, 24, 12, 45, 23, DateTimeKind.Utc),
            State = TaskState.InProgress
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/1/tasks/{_state.TaskId}", updateTaskContract);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task.Should().NotBeNull();
        task.Id.Should().Be(_state.TaskId);
        task.Name.Should().Be(updateTaskContract.Name);
        task.State.Should().Be(updateTaskContract.State);
        task.DeadLine.Should().Be(updateTaskContract.DeadLine);
        task.UserId.Should().Be(1);
    }
    
    [Fact, TestPriority(5)]
    public async Task Delete_TaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

        // Act
        var response = await _client.DeleteAsync("/api/users/1/tasks/1341263871");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact, TestPriority(5)]
    public async Task Delete_ExistingTask_DeletesAndReturnsOk()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
        
        // Act
        var response = await _client.DeleteAsync($"/api/users/1/tasks/{_state.TaskId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}