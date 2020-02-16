using AutoMapper;
using OAuth.Api.Application.Models;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Mappings
{
    public class FindUserByUsernameQueryResponse_SignInCommandResponse_Profile : Profile
    {
        public FindUserByUsernameQueryResponse_SignInCommandResponse_Profile()
        {
            CreateMap<FindUserByUsernameQueryResponse, SignInCommandResponse>()
                .ForMember(d => d.UserId, o => o.MapFrom(m => m.Id));
        }
    }
}
