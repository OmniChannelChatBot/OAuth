using AutoMapper;
using OAuth.Api.Application.Models;
using OAuth.Infrastructure.Services;

namespace OAuth.Api.Application.Mappings
{
    public class GetUserByIdQueryResponse_GetUserQueryResponse_Profile : Profile
    {
        public GetUserByIdQueryResponse_GetUserQueryResponse_Profile()
        {
            CreateMap<GetUserByIdQueryResponse, GetUserQueryResponse>()
                .ForMember(d => d.UserId, o => o.MapFrom(m => m.Id));
        }
    }
}
