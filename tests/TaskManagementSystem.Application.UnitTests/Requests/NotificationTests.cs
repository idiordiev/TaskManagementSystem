using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Notifications.Queries;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.UnitTests.Requests;

[TestFixture]
public class NotificationTests
{
    [Test]
    public async Task GetNotificationQuery_UpcomingTasksExists_ReturnsNotifications()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 24, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);

        var query = new GetNotificationsQuery { UserId = 1 };
        var handler = new GetNotificationsQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await handler.Handle(query, CancellationToken.None);

        // Assert
        notifications.Should().NotBeEmpty();
        notifications.Should().Contain(x => x.TaskId == 2);
        notifications.Should().Contain(x => x.ExpiresIn != default);
        notifications.Should().Contain(x => x.UserId == 1);
        notifications.Should().Contain(x => !string.IsNullOrEmpty(x.Message));
    }
    
    [Test]
    public async Task GetNotificationQuery_NoUpcomingTasks_ReturnsEmptyList()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 14, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetNotificationsQuery { UserId = 1 };
        var handler = new GetNotificationsQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await handler.Handle(query, CancellationToken.None);

        // Assert
        notifications.Should().BeEmpty();
    }
    
    [Test]
    public async Task GetNotificationQuery_PastTasks_ReturnsEmptyList()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 30, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TaskEntity, bool>> predicate, CancellationToken _) => UnitTestHelper.Tasks.Where(predicate.Compile()).ToList());
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetNotificationsQuery { UserId = 1 };
        var handler = new GetNotificationsQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var notifications = await handler.Handle(query, CancellationToken.None);

        // Assert
        notifications.Should().BeEmpty();
    }

    [Test]
    public async Task GetNotificationQuery_NotAdminAndWrongUser_ThrowsForbiddenException()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var mockTimeProvider = new Mock<TimeProvider>();

        mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(new DateTimeOffset(2023, 12, 24, 21, 39, 38, TimeSpan.Zero));
        mockUnitOfWork.Setup(x => x.TaskRepository.GetAsync(It.IsAny<Expression<Func<TaskEntity, bool>>>(), It.IsAny<CancellationToken>()));
        mockCurrentUserService.SetupGet(x => x.UserId).Returns(1);
        mockCurrentUserService.SetupGet(x => x.IsAdmin).Returns(false);
        
        var query = new GetNotificationsQuery { UserId = 2 };
        var handler = new GetNotificationsQueryHandler(mockUnitOfWork.Object, mockCurrentUserService.Object,
            mockTimeProvider.Object);

        // Act
        var action = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ForbiddenException>();
    }
}