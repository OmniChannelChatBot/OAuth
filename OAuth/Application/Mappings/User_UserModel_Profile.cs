﻿using AutoMapper;
using OAuth.Api.Application.Models;

namespace OAuth.Api.Application.Mappings
{
    public class User_UserModel_Profile : Profile
    {
        public User_UserModel_Profile()
        {
            CreateMap<User, UserModel>();
        }
    }
}