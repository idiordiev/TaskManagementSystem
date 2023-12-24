using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Mapping;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.UnitTests.Services;

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
    public async Task GetSubtasksByTaskIdAsync_CorrectUser_ReturnsSubtasks()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtasks = await service.GetSubtasksByTaskIdAsync(2);

        // Assert
        subtasks.Count().Should().Be(2);
        subtasks.Should().OnlyContain(x => x.TaskId == 2);
    }
    
    [Test]
    public async Task GetSubtasksByTaskIdAsync_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.GetSubtasksByTaskIdAsync(4);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task GetSubtasksByTaskIdAsync_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.GetSubtasksByTaskIdAsync(-1);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task GetByIdAsync_TaskExists_ReturnsSubtaskResponse()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await service.GetByIdAsync(2);

        // Assert
        subtask.Should().NotBeNull();
        subtask.Id.Should().Be(2);
        subtask.Name.Should().Be("Subtask two of Task two");
        subtask.State.Should().Be(TaskState.Done);
        subtask.TaskId.Should().Be(2);
    }
    
    [Test]
    public async Task GetByIdAsync_WrongUserAndNotAdmin_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await service.GetByIdAsync(2);

        // Assert
        subtask.Should().BeNull();
    }
    
    [Test]
    public async Task GetByIdAsync_SubtaskDoesNotExists_ReturnsNull()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var subtask = await service.GetByIdAsync(-1);

        // Assert
        subtask.Should().BeNull();
    }
    
    [Test]
    public async Task AddToTaskAsync_TaskExists_AddsNewSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.AddAsync(It.IsAny<SubtaskEntity>(), It.IsAny<CancellationToken>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);
        var createSubtaskContract = new CreateSubtaskContract
        {
            Name = "New subtask"
        };

        // Act
        var subtask = await service.AddToTaskAsync(1, createSubtaskContract);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.AddAsync(It.Is<SubtaskEntity>(s => s.Name == createSubtaskContract.Name
            && s.State == TaskState.Pending
            && s.Task.Id == 1),
            It.IsAny<CancellationToken>()));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        subtask.Should().NotBeNull();
        subtask.Name.Should().Be(createSubtaskContract.Name);
        subtask.State.Should().Be(TaskState.Pending);
    }
    
    [Test]
    public async Task AddToTaskAsync_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.AddToTaskAsync(4, new CreateSubtaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task AddToTaskAsync_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.TaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Tasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.AddToTaskAsync(-1, new CreateSubtaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateAsync_SubtaskExists_UpdatesSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.Update(It.IsAny<SubtaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);
        var updateSubtaskContract = new UpdateSubtaskContract
        {
            Name = "New name",
            State = TaskState.Done
        };

        // Act
        var subtask = await service.UpdateAsync(1, updateSubtaskContract);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.Update(It.Is<SubtaskEntity>(s => s.Name == updateSubtaskContract.Name
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
    public async Task UpdateAsync_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.UpdateAsync(1, new UpdateSubtaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task UpdateAsync_SubtaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action= () => service.UpdateAsync(-1, new UpdateSubtaskContract());

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeleteAsync_SubtaskExists_UpdatesSubtask()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockUnitOfWork.Setup(x => x.SubtaskRepository.Delete(It.IsAny<SubtaskEntity>()));
        mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        await service.DeleteAsync(1);

        // Assert
        mockUnitOfWork.Verify(x => x.SubtaskRepository.Delete(It.Is<SubtaskEntity>(s => s.Id == 1)));
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
    
    [Test]
    public async Task DeleteAsync_WrongUserAndNotAdmin_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(2);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => service.DeleteAsync(1);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task DeleteAsync_SubtaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        
        mockUnitOfWork.Setup(x => x.SubtaskRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => DataStub.Subtasks.FirstOrDefault(x => x.Id == id));

        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new SubtaskService(mockUnitOfWork.Object, _mapper, mockCurrentUserService.Object);

        // Act
        var action = () => service.DeleteAsync(-1);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
}