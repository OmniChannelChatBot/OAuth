using AutoMapper;
using OAuth.Api.Application.Entity;
using OAuth.Api.Application.Models;

namespace OAuth.Api.Application.MappingProfiles
{
    public class CreateModel_User_Profile : Profile
    {
        public CreateModel_User_Profile()
        {
            CreateMap<CreateUserModel, User>();
        }
    }
}
