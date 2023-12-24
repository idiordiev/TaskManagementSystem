using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Mapping;
using TaskManagementSystem.Application.Subtasks.Commands;
using TaskManagementSystem.Application.Subtasks.Queries;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests.Services;

[TestFixture]
public class SubtaskServiceTests
{
    private readonly IMapper _mapper;
    
    public SubtaskServiceTests()
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
    public async Task GetSubtasksByTaskIdQuery_CorrectUser_ReturnsSubtasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);

        var query = new GetSubtasksByTaskIdQuery { TaskId = 2 };
        var handler = new GetSubtasksByTaskIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtasks = await handler.Handle(query, CancellationToken.None);

        // Assert
        subtasks.Count().Should().Be(2);
        subtasks.Should().OnlyContain(x => x.TaskId == 2);
    }
    
    [Test]
    public async Task GetSubtasksByTaskIdQuery_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetSubtasksByTaskIdQuery { TaskId = 4 };
        var handler = new GetSubtasksByTaskIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task GetSubtasksByTaskIdQuery_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetSubtasksByTaskIdQuery { TaskId = -1 };
        var handler = new GetSubtasksByTaskIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task GetSubtaskByIdQuery_TaskExists_ReturnsSubtaskResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetSubtaskByIdQuery { SubtaskId = 2 };
        var handler = new GetSubtaskByIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await handler.Handle(query, CancellationToken.None);

        // Assert
        subtask.Should().NotBeNull();
        subtask.Id.Should().Be(2);
        subtask.Name.Should().Be("Subtask two of Task two");
        subtask.State.Should().Be(TaskState.Done);
        subtask.TaskId.Should().Be(2);
    }
    
    [Test]
    public async Task GetSubtaskByIdQuery_WrongUserAndNotAdmin_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetSubtaskByIdQuery { SubtaskId = 2 };
        var handler = new GetSubtaskByIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await handler.Handle(query, CancellationToken.None);

        // Assert
        subtask.Should().BeNull();
    }
    
    [Test]
    public async Task GetSubtaskByIdQuery_SubtaskDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetSubtaskByIdQuery { SubtaskId = -1 };
        var handler = new GetSubtaskByIdQueryHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await handler.Handle(query, CancellationToken.None);

        // Assert
        subtask.Should().BeNull();
    }
    
    [Test]
    public async Task CreateSubtaskCommand_TaskExists_AddsNewSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.AddAsync(It.IsAny<SubtaskEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);
        var command = new CreateSubtaskCommand
        {
            TaskId = 1,
            Name = "New subtask"
        };

        // Act
        var subtask = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.AddAsync(It.Is<SubtaskEntity>(s => s.Name == command.Name
            && s.State == TaskState.Pending
            && s.Task.Id == 1),
            It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        subtask.Should().NotBeNull();
        subtask.Name.Should().Be(command.Name);
        subtask.State.Should().Be(TaskState.Pending);
    }
    
    [Test]
    public async Task CreateSubtaskCommand_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new CreateSubtaskCommand { TaskId = 4 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task CreateSubtaskCommand_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new CreateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new CreateSubtaskCommand { TaskId = -1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateSubtaskCommand_SubtaskExists_UpdatesSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.Update(It.IsAny<SubtaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);
        var command = new UpdateSubtaskCommand
        {
            SubtaskId = 1,
            Name = "New name",
            State = TaskState.Done
        };

        // Act
        var subtask = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.Update(It.Is<SubtaskEntity>(s => s.Name == command.Name
                && s.State == TaskState.Done
                && s.TaskId == 2
                && s.Id == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        subtask.Should().NotBeNull();
        subtask.Id.Should().Be(1);
        subtask.Name.Should().Be("New name");
        subtask.State.Should().Be(TaskState.Done);
        subtask.TaskId.Should().Be(2);
    }
    
    [Test]
    public async Task UpdateSubtaskCommand_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new UpdateSubtaskCommand { SubtaskId = 1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateSubtaskCommand_SubtaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new UpdateSubtaskCommandHandler(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => handler.Handle(new UpdateSubtaskCommand { SubtaskId = -1 }, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeleteSubtaskCommand_SubtaskExists_UpdatesSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.Delete(It.IsAny<SubtaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteSubtaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);
        var command = new DeleteSubtaskCommand { SubtaskId = 1 };
        
        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.Delete(It.Is<SubtaskEntity>(s => s.Id == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
    
    [Test]
    public async Task DeleteSubtaskCommand_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteSubtaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);
        var command = new DeleteSubtaskCommand { SubtaskId = 1 };

        // Act
        var action = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeleteSubtaskCommand_SubtaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => UnitTestHelper.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var handler = new DeleteSubtaskCommandHandler(mockUnitOfWork.Object, mockCurrentUserService.Object);
        var command = new DeleteSubtaskCommand { SubtaskId = -1 };

        // Act
        var action = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
}