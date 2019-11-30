using AutoMapper;
using OAuth.Application.Entity;
using OAuth.Application.Models;

namespace OAuth.Application.MappingProfiles
{
    public class User_UserModel_Profile : Profile
    {
        public User_UserModel_Profile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
