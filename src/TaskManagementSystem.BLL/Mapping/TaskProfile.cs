using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.BLL.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskEntity, TaskResponse>();
        CreateMap<UpdateTaskContract, TaskEntity>()
            .ForMember(x => x.DeadLine, options => options.MapFrom(src => src.DeadLine == null ? (DateTime?)null : src.DeadLine.Value.ToUniversalTime()));
    }
}