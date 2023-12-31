﻿using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mapping;
using TaskManagementSystem.Application.Subtasks.Commands;
using TaskManagementSystem.Application.Tasks.Commands;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Application.Tasks.Queries;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests.Requests;

[TestFixture]
public class TaskServiceTests
{
    private readonly IMapper _mapper;
    
    public TaskServiceTests()
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
    public async Task GetTasksByUserIdQuery_WithoutFilters_ReturnsTasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);

        var query = new GetTasksByUserIdQuery { UserId = 1 };
        var handler = new GetTasksByUserIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await handler.Handle(query, CancellationToken.None);

        // Assert
        tasks.Count().Should().Be(3);
        tasks.Should().OnlyContain(x => x.UserId == 1);
    }

    [Test]
    public async Task GetTasksByUserIdQuery_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();

        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTasksByUserIdQuery { UserId = 2 };
        var handler = new GetTasksByUserIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Test]
    public async Task GetTasksByUserIdQuery_FilterByState_ReturnsTasksWithCorrectState()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTasksByUserIdQuery { UserId = 1, Filters = new TaskFiltersModel { States = [TaskState.Pending] }};
        var handler = new GetTasksByUserIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await handler.Handle(query, CancellationToken.None);

        // Assert
        tasks.Count().Should().Be(2);
        tasks.Should().OnlyContain(x => x.UserId == 1 && x.State == TaskState.Pending);
    }
    
    [Test]
    public async Task GetTasksByUserIdQuery_FilterByCategory_ReturnsTasksWithCorrectCategory()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTasksByUserIdQuery { UserId = 1, Filters = new TaskFiltersModel { Categories = ["cat1"] }};
        var handler = new GetTasksByUserIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await handler.Handle(query, CancellationToken.None);

        // Assert
        tasks.Count().Should().Be(2);
        tasks.Should().OnlyContain(x => x.UserId == 1 && x.Category == "cat1");
    }
    
    [Test]
    public async Task GetTaskByIdQuery_TaskExists_ReturnTaskResponseWithSubtasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTaskByIdQuery { TaskId = 2 };
        var handler = new GetTaskByIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await handler.Handle(query, CancellationToken.None);

        // Assert
        task.Should().NotBeNull();
        task.Id.Should().Be(2);
        task.Name.Should().Be("Task two");
        task.DeadLine.Should().Be(new DateTime(2023, 12, 24, 23, 59, 59));
        task.Category.Should().Be("cat1");
        task.State.Should().Be(TaskState.InProgress);
        task.UserId.Should().Be(1);
        task.Subtasks.Count().Should().Be(2);
    }
    
    [Test]
    public async Task GetTaskByIdQuery_TaskDoesNotExists_ReturnNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTaskByIdQuery { TaskId = -1 };
        var handler = new GetTaskByIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await handler.Handle(query, CancellationToken.None);

        // Assert
        task.Should().BeNull();
    }
    
    [Test]
    public async Task GetTaskByIdQuery_WrongUserAndNotAdmin_ReturnNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetTaskByIdQuery { TaskId = 4 };
        var handler = new GetTaskByIdQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await handler.Handle(query, CancellationToken.None);

        // Assert
        task.Should().BeNull();
    }
    
    [Test]
    public async Task CreateTaskCommand_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();

        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new CreateTaskCommand { UserId = 2 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Test]
    public async Task CreateTaskCommand_DeletedUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();

        mockUnitOfWork.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new CreateTaskCommand { UserId = 2 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task CreateTaskCommand_CorrectUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.TaskRepository.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        var deadLine = new DateTime(2023, 12, 25, 12, 56, 46, DateTimeKind.Utc);
        var command = new CreateTaskCommand
        {
            UserId = 1,
            Name = "New task",
            DeadLine = deadLine,
            Category = "category1",
            Subtasks = new List<CreateSubtaskCommand>
            {
                new CreateSubtaskCommand
                {
                    Name = "Subtask1"
                },
                new CreateSubtaskCommand
                {
                    Name = "Subtask2"
                }
            }
        };

        // Act
        var task = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.TaskRepository.AddAsync(It.Is<TaskEntity>(t => t.Name == command.Name
                && t.State == TaskState.Pending
                && t.DeadLine == command.DeadLine
                && t.User.Id == 1
                && t.Category == command.Category
                && t.Subtasks.Count == 2
                && t.Subtasks.Any(s => s.Name == "Subtask1")
                && t.Subtasks.Any(s => s.Name == "Subtask2")), 
            It.IsAny<CancellationToken>()));

        task.Should().NotBeNull();
        task.Name.Should().Be(command.Name);
        task.DeadLine.Should().Be(command.DeadLine);
        task.Category.Should().Be(command.Category);
        task.Subtasks.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task UpdateTaskCommand_TaskExists_UpdatesTask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.Update(It.IsAny<TaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);
        var command = new UpdateTaskCommand
        {
            TaskId = 1,
            Name = "New name",
            DeadLine = new DateTime(2023, 12, 30, 12, 14, 15, DateTimeKind.Utc),
            State = TaskState.InProgress
        };

        // Act
        var task = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.TaskRepository.Update(It.Is<TaskEntity>(t => t.Id == 1
            && t.Name == command.Name
            && t.DeadLine == command.DeadLine
            && t.State == command.State
            && t.Category == "cat1"
            && t.UserId == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        task.Should().NotBeNull();
        task.Id.Should().Be(1);
        task.Name.Should().Be(command.Name);
        task.DeadLine.Should().Be(command.DeadLine);
        task.State.Should().Be(command.State);
        task.Category.Should().Be("cat1");
        task.UserId.Should().Be(1);
    }
    
    [Test]
    public async Task UpdateTaskCommand_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new UpdateTaskCommand { TaskId = -1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateTaskCommand_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => handler.Handle(new UpdateTaskCommand { TaskId = 4 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Test]
    public async Task DeleteByIdAsync_TaskExists_DeletesTask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.Delete(It.IsAny<TaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);

        // Act
        await handler.Handle(new DeleteTaskCommand() { TaskId = 1 }, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.TaskRepository.Delete(It.Is<TaskEntity>(t => t.Id == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
    
    [Test]
    public async Task DeleteByIdAsync_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new DeleteTaskCommand { TaskId = -1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeleteByIdAsync_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteTaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new DeleteTaskCommand { TaskId = 4 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
}