using AutoMapper;
using TaskManagementSystem.Application.Users.Commands;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserResponse>();
        CreateMap<CreateUserCommand, UserEntity>();
        CreateMap<UpdateUserCommand, UserEntity>();
    }
}