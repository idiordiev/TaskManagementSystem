using FluentAssertions;
using NUnit.Framework;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Infrastructure.Persistence;
using TaskManagementSystem.Infrastructure.Persistence.Repositories;

namespace TaskManagementSystem.Infrastructure.UnitTests.Repositories;

[TestFixture]
public class UserRepositoryTests
{
    private DataContext _context;

    [SetUp]
    public void Setup()
    {
        _context = UnitTestHelper.DataContext;
    }

    [Test]
    public async Task GetAsync_WithoutPredicate_ReturnsAllUsers()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var users = await repository.GetAsync();

        // Assert
        users.Should().NotBeEmpty();
        users.Count().Should().Be(2);
        users.Should().Contain(x => x.Id == 1);
        users.Should().Contain(x => x.Id == 2);
    }

    [Test]
    public async Task GetAsync_WithPredicate_ReturnsMatchingUsers()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var users = await repository.GetAsync(x => x.Id == 2);

        // Assert
        users.Should().NotBeEmpty();
        users.Count().Should().Be(1);
        users.Should().Contain(x => x.Id == 2);
    }
    
    [Test]
    public async Task GetByIdAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var user = await repository.GetByIdAsync(2);

        // Assert
        var expected = UnitTestHelper.Users.First(x => x.Id == 2);
        
        user.Should().NotBeNull();
        user.Id.Should().Be(expected.Id);
        user.Name.Should().Be(expected.Name);
        user.Email.Should().Be(expected.Email);
        user.State.Should().Be(expected.State);
    }
    
    [Test]
    public async Task GetByIdAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var user = await repository.GetByIdAsync(-1);

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task GetByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var user = await repository.GetByEmailAsync("john.doe@mail.com");

        // Assert
        var expected = UnitTestHelper.Users.First(x => x.Email == "john.doe@mail.com");
        
        user.Should().NotBeNull();
        user.Id.Should().Be(expected.Id);
        user.Name.Should().Be(expected.Name);
        user.Email.Should().Be(expected.Email);
        user.State.Should().Be(expected.State);
    }
    
    [Test]
    public async Task GetByEmailAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var user = await repository.GetByEmailAsync("somenonexistingemail@rfhalskjf.com");

        // Assert
        user.Should().BeNull();
    }
    
    [Test]
    public async Task CheckIfActiveUserWithSameEmailExistsAsync_UserExists_ReturnsTrue()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var result = await repository.CheckIfActiveUserWithSameEmailExistsAsync("john.doe@mail.com");

        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task CheckIfActiveUserWithSameEmailExistsAsync_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var result = await repository.CheckIfActiveUserWithSameEmailExistsAsync("somenonexistingemail@rfhalskjf.com");

        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public async Task CheckIfActiveUserWithSameEmailExistsAsync_UserDeleted_ReturnsFalse()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var result = await repository.CheckIfActiveUserWithSameEmailExistsAsync("jane.doe@mail.com");

        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public async Task AddAsync_NewUser_AddsNewUser()
    {
        // Arrange
        var repository = new UserRepository(_context);

        var newUser = new UserEntity
        {
            Name = "new user",
            Email = "email@tttt.com",
            State = UserState.Active
        };

        // Act
        await repository.AddAsync(newUser);
        await _context.SaveChangesAsync();

        // Assert
        _context.Users.Count().Should().Be(3);
    }
    
    [Test]
    public async Task Update_ExistingUser_UpdatesUser()
    {
        // Arrange
        var repository = new UserRepository(_context);
        
        var newName = "newName";
        var newState = UserState.Deleted;

        // Act
        var user = _context.Users.First(x => x.Id == 1);
        user.Name = newName;
        user.State = newState;
        
        repository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        _context.Users.Count().Should().Be(2);
        _context.Users.First(x => x.Id == 1).Name.Should().Be(newName);
        _context.Users.First(x => x.Id == 1).State.Should().Be(newState);
    }
    
    [Test]
    public async Task Delete_ExistingUser_DeletesUser()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        var userToDelete = _context.Users.First(x => x.Id == 2);
        
        repository.Delete(userToDelete);
        await _context.SaveChangesAsync();

        // Assert
        _context.Users.Count().Should().Be(1);
        _context.Users.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_ExistingUser_DeletesUser()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        await repository.DeleteByIdAsync(2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Users.Count().Should().Be(1);
        _context.Users.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_NotExistingUser_NoChanges()
    {
        // Arrange
        var repository = new UserRepository(_context);

        // Act
        await repository.DeleteByIdAsync(-2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Users.Count().Should().Be(2);
    }
}