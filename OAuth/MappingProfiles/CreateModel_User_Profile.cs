using AutoMapper;
using OAuth.Entity;
using OAuth.Models;

namespace OAuth.MappingProfiles
{
    public class CreateModel_User_Profile : Profile
    {
        public CreateModel_User_Profile()
        {
            CreateMap<CreateModel, User>();
        }
    }
}
