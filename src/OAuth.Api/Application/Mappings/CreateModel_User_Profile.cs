using AutoMapper;
using OAuth.Api.Application.Models;

namespace OAuth.Api.Application.Mappings
{
    public class CreateModel_User_Profile : Profile
    {
        public CreateModel_User_Profile()
        {
            CreateMap<CreateUserModel, User>();
        }
    }
}
