using OAuth.Core.Dtos;
using System.Security.Claims;

namespace OAuth.Core.Interfaces
{
    public interface ITokenService
    {
        AccessToken GenerateAccessToken(int userId, string username);

        RefreshToken GenerateRefreshToken();

        ClaimsPrincipal GetClaimsPrincipalByExpiredToken(string accessToken);
    }
}
