using AutoMapper;
using TaskManagementSystem.Application.Subtasks.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class SubtaskProfile : Profile
{
    public SubtaskProfile()
    {
        CreateMap<SubtaskEntity, SubtaskResponse>();
    }
}