using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mapping;
using TaskManagementSystem.Application.Services;
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
    public async Task GetAllAsync_ActiveUsersExists_ReturnsActiveUsers()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Users.Where(predicate.Compile()).ToList());
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var users = await service.GetAllAsync();

        // Assert
        users.Count().Should().Be(1);
        users.Should().OnlyContain(x => x.State == UserState.Active);
    }

    [Test]
    public async Task GetByIdAsync_UserExists_ReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var user = await service.GetByIdAsync(1);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task GetByIdAsync_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var user = await service.GetByIdAsync(-1);

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task GetByEmailAsync_UserExists_ReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string email, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Email == email));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var user = await service.GetByEmailAsync("john.doe@mail.com");

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task GetByEmailAsync_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string email, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Email == email));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var user = await service.GetByEmailAsync("somenotexistingemail@somedomain.com");

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task AddAsync_UserWithEmailExists_AddUserAndCreatesAccount()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockUnitOfWork.Setup(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.CreateAccountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        var createUserContract = new CreateUserContract
        {
            Name = "NewName",
            Email = "name@mail.com",
            Password = "SoMePss22aaa"
        };
        
        // Act
        var user = await service.CreateAsync(createUserContract);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.AddAsync(It.Is<UserEntity>(u => u.Email == createUserContract.Email && u.Name == createUserContract.Name), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Verify(x => x.CreateAccountAsync(It.Is<string>(e => e == createUserContract.Email),
            It.Is<string>(p => p == createUserContract.Password), It.IsAny<int>()));
        
        user.Should().NotBeNull();
        user.Name.Should().Be(createUserContract.Name);
        user.Email.Should().Be(createUserContract.Email);
    }
    
    [Test]
    public async Task AddAsync_UserWithEmailExists_ThrowsUserExistsException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var action = () => service.CreateAsync(new CreateUserContract());

        // Assert
        await action.Should().ThrowAsync<UserExistsException>();
    }
    
    [Test]
    public async Task AddAsync_IdentityServiceFails_DeletesUserAndRethrows()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockUnitOfWork.Setup(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.CreateAccountAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);

        // Act
        var action = () => service.CreateAsync(new CreateUserContract());

        // Assert
        await action.Should().ThrowAsync<Exception>();
        
        mockUnitOfWork.Verify(x => x.UserRepository.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.UserRepository.Delete(It.IsAny<UserEntity>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task UpdateAsync_UserExists_UpdatesAndReturnsUserResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.UserRepository.Update(It.IsAny<UserEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        var updateUserContract = new UpdateUserContract
        {
            Name = "TEst111"
        };

        // Act
        var user = await service.UpdateAsync(1, updateUserContract);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.Update(It.Is<UserEntity>(u => u.Id == 1 && u.Name == updateUserContract.Name)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        
        user.Should().NotBeNull();
        user.Id.Should().Be(1);
        user.Name.Should().Be("TEst111");
        user.State.Should().Be(UserState.Active);
        user.Email.Should().Be("john.doe@mail.com");
    }

    [Test]
    public async Task UpdateAsync_UserDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        
        var updateUserContract = new UpdateUserContract
        {
            Name = "TEst111"
        };

        // Act
        var action = () => service.UpdateAsync(-1, updateUserContract);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeactivateAsync_UserExists_SetsAsDeletedAndDeletesAccounts()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.UserRepository.Update(It.IsAny<UserEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Setup(x => x.DeleteAccountsForUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        
        // Act
        await service.DeactivateAsync(1);

        // Assert
        mockUnitOfWork.Verify(x => x.UserRepository.Update(It.Is<UserEntity>(u => u.Id == 1 && u.State == UserState.Deleted)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        mockIdentityService.Verify(x => x.DeleteAccountsForUserAsync(1, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task DeactivateAsync_UserDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockIdentityService = new Mock<IIdentityService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        
        var service = new UserService(mockUnitOfWork.Object, mockIdentityService.Object, _mapper);
        
        // Act
        var action = () => service.DeactivateAsync(-1);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
}