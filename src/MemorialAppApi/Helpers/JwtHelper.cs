using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace MemorialAppApi.Helpers;

public static class JwtHelper
{
    /// <summary>
    /// Extracts the user ID from the JWT token in the Authorization header
    /// </summary>
    /// <param name="req">The HTTP request data</param>
    /// <returns>User ID if found, otherwise Guid.Empty</returns>
    public static Guid ExtractUserIdFromToken(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            return Guid.Empty;
        }

        var authHeader = authHeaders.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Guid.Empty;
        }

        try
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => 
                c.Type == "nameid" || 
                c.Type == "userId" || 
                c.Type == ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                return parsedUserId;
            }
        }
        catch
        {
            // Return empty GUID if token parsing fails
            return Guid.Empty;
        }

        return Guid.Empty;
    }

    /// <summary>
    /// Extracts the user ID from the JWT token in the Authorization header as nullable Guid
    /// </summary>
    /// <param name="req">The HTTP request data</param>
    /// <returns>User ID if found, otherwise null</returns>
    public static Guid? ExtractUserIdFromTokenNullable(HttpRequestData req)
    {
        var userId = ExtractUserIdFromToken(req);
        return userId != Guid.Empty ? userId : null;
    }
}
