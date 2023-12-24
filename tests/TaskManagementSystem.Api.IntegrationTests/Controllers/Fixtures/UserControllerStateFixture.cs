using TaskManagementSystem.Application.Users.Commands;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers.Fixtures;

public class UserControllerStateFixture
{
    public int UserId { get; set; } = -1;

    public CreateUserCommand CreateUserCommand { get; } = new CreateUserCommand
    {
        Name = Guid.NewGuid().ToString(),
        Email = $"{Guid.NewGuid().ToString()}@mail.com",
        Password = Guid.NewGuid().ToString(),
    };
}