using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Mapping;
using TaskManagementSystem.BLL.Models;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.UnitTests.Services;

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
    public async Task GetTasksByUserIdAsync_WithoutFilters_ReturnsTasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await service.GetTasksByUserIdAsync(1, new TaskFiltersModel());

        // Assert
        tasks.Count().Should().Be(3);
        tasks.Should().OnlyContain(x => x.UserId == 1);
    }

    [Test]
    public async Task GetTasksByUserIdAsync_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.GetTasksByUserIdAsync(2, new TaskFiltersModel());

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Test]
    public async Task GetTasksByUserIdAsync_FilterByState_ReturnsTasksWithCorrectState()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await service.GetTasksByUserIdAsync(1, new TaskFiltersModel { States = [TaskState.Pending] });

        // Assert
        tasks.Count().Should().Be(2);
        tasks.Should().OnlyContain(x => x.UserId == 1 && x.State == TaskState.Pending);
    }
    
    [Test]
    public async Task GetTasksByUserIdAsync_FilterByCategory_ReturnsTasksWithCorrectCategory()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var tasks = await service.GetTasksByUserIdAsync(1, new TaskFiltersModel { Categories = ["cat1"] });

        // Assert
        tasks.Count().Should().Be(2);
        tasks.Should().OnlyContain(x => x.UserId == 1 && x.Category == "cat1");
    }
    
    [Test]
    public async Task GetByIdAsync_TaskExists_ReturnTaskResponseWithSubtasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await service.GetByIdAsync(2);

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
    public async Task GetByIdAsync_TaskDoesNotExists_ReturnNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await service.GetByIdAsync(-1);

        // Assert
        task.Should().BeNull();
    }
    
    [Test]
    public async Task GetByIdAsync_WrongUserAndNotAdmin_ReturnNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var task = await service.GetByIdAsync(4);

        // Assert
        task.Should().BeNull();
    }
    
    [Test]
    public async Task AddAsync_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.AddAsync(2, new CreateTaskContract());

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Test]
    public async Task AddAsync_DeletedUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserEntity, bool>> predicate, CancellationToken _) => DataStub.Users.Where(predicate.Compile()).ToList());
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.AddAsync(2, new CreateTaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task AddAsync_CorrectUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Users.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());

        mockUnitOfWork.Setup(x => x.TaskRepository.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        var deadLine = new DateTime(2023, 12, 25, 12, 56, 46, DateTimeKind.Utc);
        var createTaskContract = new CreateTaskContract()
        {
            Name = "New task",
            DeadLine = deadLine,
            Category = "category1",
            Subtasks = new List<CreateSubtaskContract>
            {
                new CreateSubtaskContract
                {
                    Name = "Subtask1"
                },
                new CreateSubtaskContract
                {
                    Name = "Subtask2"
                }
            }
        };

        // Act
        var newTask = await service.AddAsync(1, createTaskContract);

        // Assert
        mockUnitOfWork.Verify(x => x.TaskRepository.AddAsync(It.Is<TaskEntity>(t => t.Name == createTaskContract.Name
                && t.State == TaskState.Pending
                && t.DeadLine == createTaskContract.DeadLine
                && t.User.Id == 1
                && t.Category == createTaskContract.Category
                && t.Subtasks.Count == 2
                && t.Subtasks.Any(s => s.Name == "Subtask1")
                && t.Subtasks.Any(s => s.Name == "Subtask2")), 
            It.IsAny<CancellationToken>()));

        newTask.Should().NotBeNull();
        newTask.Name.Should().Be(createTaskContract.Name);
        newTask.DeadLine.Should().Be(createTaskContract.DeadLine);
        newTask.Category.Should().Be(createTaskContract.Category);
        newTask.Subtasks.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task UpdateAsync_TaskExists_UpdatesTask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.Update(It.IsAny<TaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        var updateTaskContract = new UpdateTaskContract
        {
            Name = "New name",
            DeadLine = new DateTime(2023, 12, 30, 12, 14, 15, DateTimeKind.Utc),
            State = TaskState.InProgress
        };

        // Act
        var task = await service.UpdateAsync(1, updateTaskContract);

        // Assert
        mockUnitOfWork.Verify(x => x.TaskRepository.Update(It.Is<TaskEntity>(t => t.Id == 1
            && t.Name == updateTaskContract.Name
            && t.DeadLine == updateTaskContract.DeadLine
            && t.State == updateTaskContract.State
            && t.Category == "cat1"
            && t.UserId == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        task.Should().NotBeNull();
        task.Id.Should().Be(1);
        task.Name.Should().Be(updateTaskContract.Name);
        task.DeadLine.Should().Be(updateTaskContract.DeadLine);
        task.State.Should().Be(updateTaskContract.State);
        task.Category.Should().Be("cat1");
        task.UserId.Should().Be(1);
    }
    
    [Test]
    public async Task UpdateAsync_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.UpdateAsync(-1, new UpdateTaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateAsync_WrongUserAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.UpdateAsync(4, new UpdateTaskContract());

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
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        mockUnitOfWork.Setup(x => x.TaskRepository.Delete(It.IsAny<TaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        await service.DeleteByIdAsync(1);

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
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.DeleteByIdAsync(-1);

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
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new TaskService(mockUnitOfWork.Object, mockCurrentUserService.Object, _mapper);

        // Act
        var action = () => service.DeleteByIdAsync(4);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
}