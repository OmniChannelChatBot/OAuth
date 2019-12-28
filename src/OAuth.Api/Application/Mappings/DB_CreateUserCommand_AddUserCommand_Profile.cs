using AutoMapper;
using OAuth.Api.Application.Commands;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Mappings
{
    public class DB_CreateUserCommand_AddUserCommand_Profile : Profile
    {
        public DB_CreateUserCommand_AddUserCommand_Profile()
        {
            CreateMap<CreateUserCommand, AddUserCommand>()
                .ForMember(d => d.Type, o => o.MapFrom(m => (int)m.Type));
        }
    }
}
