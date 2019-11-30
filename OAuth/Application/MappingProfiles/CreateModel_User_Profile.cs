using AutoMapper;
using OAuth.Application.Entity;
using OAuth.Application.Models;

namespace OAuth.Application.MappingProfiles
{
    public class CreateModel_User_Profile : Profile
    {
        public CreateModel_User_Profile()
        {
            CreateMap<CreateUserModel, User>();
        }
    }
}
