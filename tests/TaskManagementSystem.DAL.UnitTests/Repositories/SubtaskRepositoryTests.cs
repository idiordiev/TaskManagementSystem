using FluentAssertions;
using NUnit.Framework;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Repositories;

namespace TaskManagementSystem.DAL.UnitTests.Repositories;

[TestFixture]
public class SubtaskRepositoryTests
{
    private DataContext _context;

    [SetUp]
    public void Setup()
    {
        _context = UnitTestHelper.DataContext;
    }

    [Test]
    public async Task GetAsync_WithoutPredicate_ReturnsAllSubtasks()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        var subtasks = await repository.GetAsync();

        // Assert
        subtasks.Should().NotBeEmpty();
        subtasks.Count().Should().Be(3);
        subtasks.Should().Contain(x => x.Id == 1);
        subtasks.Should().Contain(x => x.Id == 2);
        subtasks.Should().Contain(x => x.Id == 3);
    }

    [Test]
    public async Task GetAsync_WithPredicate_ReturnsMatchingSubtasks()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        var subtasks = await repository.GetAsync(x => x.Id == 2);

        // Assert
        subtasks.Should().NotBeEmpty();
        subtasks.Count().Should().Be(1);
        subtasks.Should().Contain(x => x.Id == 2);
    }
    
    [Test]
    public async Task GetByIdAsync_TaskExists_ReturnsTask()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        var subtask = await repository.GetByIdAsync(2);

        // Assert
        var expected = UnitTestHelper.Subtasks.First(x => x.Id == 2);
        
        subtask.Should().NotBeNull();
        subtask.Id.Should().Be(expected.Id);
        subtask.Name.Should().Be(expected.Name);
        subtask.State.Should().Be(expected.State);
    }
    
    [Test]
    public async Task GetByIdAsync_TaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        var subtask = await repository.GetByIdAsync(-1);

        // Assert
        subtask.Should().BeNull();
    }
    
    [Test]
    public async Task AddAsync_NewTask_AddsNewTask()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        var newSubtask = new SubtaskEntity
        {
            Name = "new task",
            State = TaskState.InProgress,
            TaskId = 1
        };

        // Act
        await repository.AddAsync(newSubtask);
        await _context.SaveChangesAsync();

        // Assert
        _context.Subtasks.Count().Should().Be(4);
    }
    
    [Test]
    public async Task Update_ExistingTask_UpdatesTask()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);
        
        var newName = "newNameSubtask";
        var newState = TaskState.Done;

        // Act
        var subtask = _context.Subtasks.First(x => x.Id == 1);
        subtask.Name = newName;
        subtask.State = newState;
        
        repository.Update(subtask);
        await _context.SaveChangesAsync();

        // Assert
        _context.Subtasks.Count().Should().Be(3);
        _context.Subtasks.First(x => x.Id == 1).Name.Should().Be(newName);
        _context.Subtasks.First(x => x.Id == 1).State.Should().Be(newState);
    }
    
    [Test]
    public async Task Delete_ExistingTask_DeletesTask()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        var subtaskToDelete = _context.Subtasks.First(x => x.Id == 2);
        
        repository.Delete(subtaskToDelete);
        await _context.SaveChangesAsync();

        // Assert
        _context.Subtasks.Count().Should().Be(2);
        _context.Subtasks.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_ExistingTask_DeletesTask()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        await repository.DeleteByIdAsync(2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Subtasks.Count().Should().Be(2);
        _context.Subtasks.FirstOrDefault(x => x.Id == 2).Should().BeNull();
    }
    
    [Test]
    public async Task DeleteByIdAsync_NotExistingTask_NoChanges()
    {
        // Arrange
        var repository = new SubtaskRepository(_context);

        // Act
        await repository.DeleteByIdAsync(-2);
        await _context.SaveChangesAsync();

        // Assert
        _context.Subtasks.Count().Should().Be(3);
    }
}