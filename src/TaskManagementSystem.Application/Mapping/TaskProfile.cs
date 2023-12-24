using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskEntity, TaskResponse>();
        CreateMap<UpdateTaskContract, TaskEntity>()
            .ForMember(x => x.DeadLine, options => options.MapFrom(src => src.DeadLine == null ? (DateTime?)null : src.DeadLine.Value.ToUniversalTime()));
    }
}