using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.UnitTests;

public static class UnitTestHelper
{
    public static List<UserEntity> Users =>
    [
        new UserEntity
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@mail.com",
            State = UserState.Active
        },

        new UserEntity
        {
            Id = 2,
            Name = "Jane Doe",
            Email = "jane.doe@mail.com",
            State = UserState.Deleted
        }
    ];

    public static List<TaskEntity> Tasks =>
    [
        new TaskEntity
        {
            Id = 1,
            Name = "Task one",
            DeadLine = null,
            Category = "cat1",
            State = TaskState.Pending,
            UserId = 1,
        },
        new TaskEntity
        {
            Id = 2,
            Name = "Task two",
            DeadLine = new DateTime(2023, 12, 24, 23, 59, 59),
            Category = "cat1",
            State = TaskState.InProgress,
            UserId = 1,
            Subtasks = new List<SubtaskEntity>
            {
                new SubtaskEntity
                {
                    Id = 1,
                    Name = "Subtask one of Task two",
                    State = TaskState.Pending,
                    TaskId = 2
                },
                new SubtaskEntity
                {
                    Id = 2,
                    Name = "Subtask two of Task two",
                    State = TaskState.Done,
                    TaskId = 2
                }
            }
        },
        new TaskEntity
        {
            Id = 3,
            Name = "Task three",
            DeadLine = new DateTime(2023, 12, 27, 23, 59, 59),
            Category = "cat2",
            State = TaskState.Pending,
            UserId = 1,
            Subtasks = new List<SubtaskEntity>
            {
                new SubtaskEntity
                {
                    Id = 3,
                    Name = "Subtask one of Task three",
                    State = TaskState.Pending,
                    TaskId = 3
                }
            }
        },
        new TaskEntity
        {
            Id = 4,
            Name = "Task four",
            DeadLine = null,
            Category = "cat2",
            State = TaskState.Done,
            UserId = 2
        }
    ];

    public static List<SubtaskEntity> Subtasks =>
    [
        new SubtaskEntity
        {
            Id = 1,
            Name = "Subtask one of Task two",
            State = TaskState.Pending,
            TaskId = 2,
            Task = new TaskEntity
            {
                Id = 2,
                Name = "Task two",
                DeadLine = new DateTime(2023, 12, 24, 23, 59, 59),
                Category = "cat1",
                State = TaskState.InProgress,
                UserId = 1
            }
        },

        new SubtaskEntity
        {
            Id = 2,
            Name = "Subtask two of Task two",
            State = TaskState.Done,
            TaskId = 2,
            Task = new TaskEntity
            {
                Id = 2,
                Name = "Task two",
                DeadLine = new DateTime(2023, 12, 24, 23, 59, 59),
                Category = "cat1",
                State = TaskState.InProgress,
                UserId = 1
            }
        },

        new SubtaskEntity
        {
            Id = 3,
            Name = "Subtask one of Task three",
            State = TaskState.Pending,
            TaskId = 3,
            Task = new TaskEntity
            {
                Id = 3,
                Name = "Task three",
                DeadLine = new DateTime(2023, 12, 27, 23, 59, 59),
                Category = "cat2",
                State = TaskState.Pending,
                UserId = 1
            }
        }
    ];
}