using AutoMapper;
using OAuth.Entity;
using OAuth.Models;

namespace OAuth.MappingProfiles
{
    public class User_UserModel_Profile : Profile
    {
        public User_UserModel_Profile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
