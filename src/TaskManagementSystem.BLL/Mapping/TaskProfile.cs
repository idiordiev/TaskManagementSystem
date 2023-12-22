using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using Task = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.BLL.Mapping;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<Task, TaskResponse>();
        CreateMap<UpdateTaskContract, Task>()
            .ForMember(x => x.DeadLine, options => options.MapFrom(src => src.DeadLine == null ? (DateTime?)null : src.DeadLine.Value.ToUniversalTime()));
    }
}