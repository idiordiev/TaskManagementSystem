using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mapping;
using TaskManagementSystem.Application.Users.Commands;
using TaskManagementSystem.Application.Users.Queries;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests.Services;

[TestFixture]
public class UserServiceTests
{
    private readonly IMapper _mapper;
    
    public UserServiceTests()
    {
        _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<TaskProfile>();
                cfg.AddProfile<SubtaskProfile>();
            })
            .CreateMapper();
    }
    
    [Test]
    public async Task GetActiveUsersQuery_ActiveUsersExists_ReturnsActiveUsers()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Users.Where(predicate.Compile()).ToList());

        var query = new GetActiveUsersQuery();
        var handler = new GetActiveUsersQueryHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var users = await handler.Handle(query, CancellationToken.None);

        // Assert
        users.Count().Should().Be(1);
        users.Should().OnlyContain(x => x.State == UserState.Active);
    }

    [Test]
    public async Task GetUserByIdQuery_UserExists_ReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var query = new GetUserByIdQuery { UserId = 1 };
        var handler = new GetUserByIdQueryHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var user = await handler.Handle(query, CancellationToken.None);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task GetUserByIdQuery_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var query = new GetUserByIdQuery { UserId = -1 };
        var handler = new GetUserByIdQueryHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var user = await handler.Handle(query, CancellationToken.None);

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task GetUserByEmailQuery_UserExists_ReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string email, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Email == email));
        
        var query = new GetUserByEmailQuery { Email = "john.doe@mail.com" };
        var handler = new GetUserByEmailQueryHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var user = await handler.Handle(query, CancellationToken.None);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task GetUserByEmailQuery_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string email, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Email == email));
        
        var query = new GetUserByEmailQuery { Email = "somenotexistingemail@somedomain.com" };
        var handler = new GetUserByEmailQueryHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var user = await handler.Handle(query, CancellationToken.None);

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task CreateUserCommand_UserWithEmailExists_AddUserAndCreatesAccount()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockUnitOfWork.Setup(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.CreateAccountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        
        var command = new CreateUserCommand
        {
            Name = "NewName",
            Email = "name@mail.com",
            Password = "SoMePss22aaa"
        };
        var handler = new CreateUserCommandHandler(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        
        // Act
        var user = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.AddAsync(It.Is<UserEntity>(u => u.Email == command.Email && u.Name == command.Name), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Verify(x => x.CreateAccountAsync(It.Is<string>(e => e == command.Email),
            It.Is<string>(p => p == command.Password), It.IsAny<int>()));
        
        user.Should().NotBeNull();
        user.Name.Should().Be(command.Name);
        user.Email.Should().Be(command.Email);
    }
    
    [Test]
    public async Task CreateUserCommand_UserWithEmailExists_ThrowsUserExistsException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateUserCommandHandler(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new CreateUserCommand(), CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<UserExistsException>();
    }
    
    [Test]
    public async Task CreateUserCommand_IdentityServiceFails_DeletesUserAndRethrows()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockUnitOfWork.Setup(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.CreateAccountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        
        var handler = new CreateUserCommandHandler(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new CreateUserCommand(), CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>();
        
        mockUnitOfWork.Verify(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.UserRepository.Delete(It.IsAny<UserEntity>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task UpdateUserCommand_UserExists_UpdatesAndReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.UserRepository.Update(It.IsAny<UserEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        var handler = new UpdateUserCommandHandler(mockUnitOfWork.Object, _mapper);
        var command = new UpdateUserCommand
        {
            Id = 1,
            Name = "TEst111"
        };

        // Act
        var user = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.Update(It.Is<UserEntity>(u => u.Id == 1 && u.Name == command.Name)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("TEst111");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task UpdateUserCommand_UserDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var handler = new UpdateUserCommandHandler(mockUnitOfWork.Object, _mapper);

        // Act
        var action = () => handler.Handle(new UpdateUserCommand(), CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeactivateUserCommand_UserExists_SetsAsDeletedAndDeletesAccounts()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.UserRepository.Update(It.IsAny<UserEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.DeleteAccountsForUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        
        var handler = new DeactivateUserCommandHandler(mockUnitOfWork.Object, mockIdentityService.Object);
        
        // Act
        await handler.Handle(new DeactivateUserCommand { UserId = 1 }, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.Update(It.Is<UserEntity>(u => u.Id == 1 && u.State == UserState.Deleted)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Verify(x => x.DeleteAccountsForUserAsync(1, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task DeactivateUserCommand_UserDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var handler = new DeactivateUserCommandHandler(mockUnitOfWork.Object, mockIdentityService.Object);
        
        // Act
        var action = () => handler.Handle(new DeactivateUserCommand { UserId = -1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
}