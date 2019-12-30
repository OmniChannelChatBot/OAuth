using AutoMapper;
using OAuth.Api.Application.Commands;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Mappings
{
    public class DB_SignUpCommand_AddUserCommand_Profile : Profile
    {
        public DB_SignUpCommand_AddUserCommand_Profile()
        {
            CreateMap<SignUpCommand, AddUserCommand>()
                .ForMember(d => d.Type, o => o.MapFrom(m => (int)m.Type))
                .ForMember(d => d.PasswordHash, o => o.MapFrom((s, d, m, rc) => rc.Items[nameof(AddUserCommand.PasswordHash)]))
                .ForMember(d => d.PasswordSalt, o => o.MapFrom((s, d, m, rc) => rc.Items[nameof(AddUserCommand.PasswordSalt)]));
        }
    }
}
