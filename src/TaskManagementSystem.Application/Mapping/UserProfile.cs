using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserResponse>();
        CreateMap<CreateUserContract, UserEntity>();
        CreateMap<UpdateUserContract, UserEntity>();
    }
}