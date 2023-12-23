using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.BLL.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserResponse>();
        CreateMap<CreateUserContract, UserEntity>();
        CreateMap<UpdateUserContract, UserEntity>();
    }
}