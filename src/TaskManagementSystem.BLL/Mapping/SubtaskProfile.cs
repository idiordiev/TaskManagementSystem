﻿using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.BLL.Mapping;

public class SubtaskProfile : Profile
{
    public SubtaskProfile()
    {
        CreateMap<SubtaskEntity, SubtaskResponse>();
        CreateMap<UpdateSubtaskContract, SubtaskEntity>();
    }
}