using System.Security.Claims;

namespace OAuth.Core.Interfaces
{
    public interface ISecurityTokenService
    {
        string GenerateAccessToken(ClaimsIdentity claimsIdentity);

        string GenerateRefreshToken();

        ClaimsPrincipal GetClaimsPrincipalByExpiredToken(string accessToken);
    }
}
