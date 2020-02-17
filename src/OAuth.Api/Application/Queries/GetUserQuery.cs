using MediatR;
using OAuth.Api.Application.Models;

namespace OAuth.Api.Application.Queries
{
    public class GetUserQuery : IRequest<GetUserQueryResponse>
    {
    }
}
