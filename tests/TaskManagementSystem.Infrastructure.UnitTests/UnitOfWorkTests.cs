using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Infrastructure.Persistence;

namespace TaskManagementSystem.Infrastructure.UnitTests;

[TestFixture]
public class UnitOfWorkTests
{
    private DataContext _context;

    [SetUp]
    public void Setup()
    {
        _context = UnitTestHelper.DataContext;
    }

    [Test]
    public void UserRepository_SingleCall_ReturnsInstanceOfUserRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var userRepository = unitOfWork.UserRepository;

        // Assert
        userRepository.Should().NotBeNull();
    }
    
    [Test]
    public void UserRepository_TwoCalls_ReturnsSameInstanceOfUserRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var userRepositoryFirst = unitOfWork.UserRepository;
        var userRepositorySecond = unitOfWork.UserRepository;

        // Assert
        userRepositoryFirst.Should().BeSameAs(userRepositorySecond);
    }
    
    [Test]
    public void TaskRepository_SingleCall_ReturnsInstanceOfTaskRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var taskRepository = unitOfWork.TaskRepository;

        // Assert
        taskRepository.Should().NotBeNull();
    }
    
    [Test]
    public void TaskRepository_TwoCalls_ReturnsSameInstanceOfTaskRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var taskRepositoryFirst = unitOfWork.TaskRepository;
        var taskRepositorySecond = unitOfWork.TaskRepository;

        // Assert
        taskRepositoryFirst.Should().BeSameAs(taskRepositorySecond);
    }
    
    [Test]
    public void SubtaskRepository_SingleCall_ReturnsInstanceOfSubtaskRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var subtaskRepository = unitOfWork.SubtaskRepository;

        // Assert
        subtaskRepository.Should().NotBeNull();
    }
    
    [Test]
    public void SubtaskRepository_TwoCalls_ReturnsSameInstanceOfSubtaskRepository()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context);

        // Act
        var subtaskRepositoryFirst = unitOfWork.SubtaskRepository;
        var subtaskRepositorySecond = unitOfWork.SubtaskRepository;

        // Assert
        subtaskRepositoryFirst.Should().BeSameAs(subtaskRepositorySecond);
    }

    [Test]
    public async Task SaveChangesAsync_OneCall_CallsMethodOfContext()
    {
        // Arrange
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var unitOfWork = new UnitOfWork(mockContext.Object);
        
        // Act
        await unitOfWork.SaveChangesAsync();

        // Assert
        mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Test]
    public void SaveChanges_OneCall_CallsMethodOfContext()
    {
        // Arrange
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(x => x.SaveChanges());

        var unitOfWork = new UnitOfWork(mockContext.Object);
        
        // Act
        unitOfWork.SaveChanges();

        // Assert
        mockContext.Verify(x => x.SaveChanges());
    }
}