using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Mapping;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.UnitTests;

[TestFixture]
public class NotificationServiceTests
{
    [Test]
    public async Task GetNotificationAsync_UpcomingTasksExists_ReturnsNotifications()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 24, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new NotificationService(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await service.GetNotificationsAsync(1);

        // Assert
        notifications.Should().NotBeEmpty();
        notifications.Should().Contain(x => x.TaskId == 2);
    }
    
    [Test]
    public async Task GetNotificationAsync_NoUpcomingTasks_ReturnsEmptyList()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 14, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new NotificationService(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await service.GetNotificationsAsync(1);

        // Assert
        notifications.Should().BeEmpty();
    }
    
    [Test]
    public async Task GetNotificationAsync_PastTasks_ReturnsEmptyList()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 30, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => DataStub.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new NotificationService(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await service.GetNotificationsAsync(1);

        // Assert
        notifications.Should().BeEmpty();
    }

    [Test]
    public async Task GetNotificationAsync_NotAdminAndWrongUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 24, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var service = new NotificationService(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var action = () => service.GetNotificationsAsync(2);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
}