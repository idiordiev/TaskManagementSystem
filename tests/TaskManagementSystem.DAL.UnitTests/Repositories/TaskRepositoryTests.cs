using FluentAssertions;
using NUnit.Framework;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Repositories;

namespace TaskManagementSystem.DAL.UnitTests.Repositories;

[TestFixture]
public class TaskRepositoryTests
{
    private DataContext _context;

    [SetUp]
    public void Setup()
    {
        _context = UnitTestHelper.DataContext;
    }

    [Test]
    public async Task GetAsync_WithoutPredicate_ReturnsAllTasks()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        var tasks = await repository.GetAsync();

        // Assert
        tasks.Should().NotBeEmpty();
        tasks.Count().Should().Be(4);
        tasks.Should().Contain(x => x.Id == 1);
        tasks.Should().Contain(x => x.Id == 2);
        tasks.Should().Contain(x => x.Id == 3);
        tasks.Should().Contain(x => x.Id == 4);
    }

    [Test]
    public async Task GetAsync_WithPredicate_ReturnsMatchingTasks()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        var tasks = await repository.GetAsync(x => x.Id == 2);

        // Assert
        tasks.Should().NotBeEmpty();
        tasks.Count().Should().Be(1);
        tasks.Should().Contain(x => x.Id == 2);
    }
    
    [Test]
    public async Task GetByIdAsync_TaskExists_ReturnsTask()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        var task = await repository.GetByIdAsync(2);

        // Assert
        var expected = UnitTestHelper.Tasks.First(x => x.Id == 2);
        
        task.Should().NotBeNull();
        task.Id.Should().Be(expected.Id);
        task.Name.Should().Be(expected.Name);
        task.State.Should().Be(expected.State);
        task.DeadLine.Should().Be(expected.DeadLine);
        task.Category.Should().Be(expected.Category);
        task.UserId.Should().Be(expected.UserId);
    }
    
    [Test]
    public async Task GetByIdAsync_TaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        var task = await repository.GetByIdAsync(-1);

        // Assert
        task.Should().BeNull();
    }
    
    [Test]
    public async Task AddAsync_NewTask_AddsNewTask()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        var newTask = new TaskEntity
        {
            Name = "new task",
            State = TaskState.InProgress,
            DeadLine = null,
            Category = "test",
            UserId = 1
        };

        // Act
        await repository.AddAsync(newTask);
        await _context.SaveChangesAsync();

        // Assert
        _context.Tasks.Count().Should().Be(5);
    }
    
    [Test]
    public async Task Update_ExistingTask_UpdatesTask()
    {
        // Arrange
        var repository = new TaskRepository(_context);
        
        var newName = "newName";
        var newState = TaskState.Done;

        // Act
        var task = _context.Tasks.First(x => x.Id == 1);
        task.Name = newName;
        task.State = newState;
        
        repository.Update(task);
        await _context.SaveChangesAsync();

        // Assert
        _context.Tasks.Count().Should().Be(4);
        _context.Tasks.First(x => x.Id == 1).Name.Should().Be(newName);
        _context.Tasks.First(x => x.Id == 1).State.Should().Be(newState);
    }
    
    [Test]
    public async Task Delete_ExistingTask_DeletesTask()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        var taskToDelete = _context.Tasks.First(x => x.Id == 2);
        
        repository.Delete(taskToDelete);
        await _context.SaveChangesAsync();

        // Assert
        _context.Tasks.Count().Should().Be(3);
        _context.Tasks.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_ExistingTask_DeletesTask()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        await repository.DeleteByIdAsync(2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Tasks.Count().Should().Be(3);
        _context.Tasks.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_NotExistingTask_NoChanges()
    {
        // Arrange
        var repository = new TaskRepository(_context);

        // Act
        await repository.DeleteByIdAsync(-2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Tasks.Count().Should().Be(4);
    }
}