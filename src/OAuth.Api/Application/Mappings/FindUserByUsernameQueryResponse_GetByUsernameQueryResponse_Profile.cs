using AutoMapper;
using OAuth.Api.Application.Models;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Mappings
{
    public class FindUserByUsernameQueryResponse_GetByUsernameQueryResponse_Profile : Profile
    {
        public FindUserByUsernameQueryResponse_GetByUsernameQueryResponse_Profile()
        {
            CreateMap<FindUserByUsernameQueryResponse, GetByUsernameQueryResponse>()
                .ForMember(d => d.UserId, o => o.MapFrom(m => m.Id));
        }
    }
}
