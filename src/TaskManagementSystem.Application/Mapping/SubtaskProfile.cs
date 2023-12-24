using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class SubtaskProfile : Profile
{
    public SubtaskProfile()
    {
        CreateMap<SubtaskEntity, SubtaskResponse>();
        CreateMap<UpdateSubtaskContract, SubtaskEntity>();
    }
}