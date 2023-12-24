using TaskManagementSystem.BLL.Contracts;

namespace TaskManagementSystem.Api.IntegrationTests.Controllers.Fixtures;

public class UserControllerStateFixture
{
    public int UserId { get; set; } = -1;

    public CreateUserContract CreateUserContract { get; } = new CreateUserContract
    {
        Name = Guid.NewGuid().ToString(),
        Email = $"{Guid.NewGuid().ToString()}@mail.com",
        Password = Guid.NewGuid().ToString(),
    };
}