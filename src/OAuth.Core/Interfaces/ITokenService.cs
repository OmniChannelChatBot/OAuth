using OAuth.Core.Dtos;
using System.Security.Claims;

namespace OAuth.Core.Interfaces
{
    public interface ITokenService
    {
        AccessToken GenerateAccessToken(int userId, string username, int refreshTokenId);

        RefreshToken GenerateRefreshToken();

        bool VerifyExpiredAccessToken(string accessToken);

        ClaimsPrincipal GetClaimsPrincipalByExpiredAccessToken(string accessToken);
    }
}
