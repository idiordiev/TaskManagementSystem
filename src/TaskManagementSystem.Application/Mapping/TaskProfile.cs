using AutoMapper;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskEntity, TaskResponse>();
    }
}