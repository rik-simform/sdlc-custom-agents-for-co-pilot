#nullable enable

namespace MyProject.Web.Services;

/// <summary>
/// Stores and retrieves JWT tokens from the HTTP session.
/// </summary>
public class TokenService(IHttpContextAccessor httpContextAccessor)
{
    private const string AccessTokenKey = "AccessToken";
    private const string RefreshTokenKey = "RefreshToken";
    private const string UserEmailKey = "UserEmail";

    private ISession Session =>
        httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("No active HTTP session.");

    /// <summary>Saves the access token, refresh token, and user email to session.</summary>
    public void StoreTokens(string accessToken, string refreshToken, string email)
    {
        Session.SetString(AccessTokenKey, accessToken);
        Session.SetString(RefreshTokenKey, refreshToken);
        Session.SetString(UserEmailKey, email);
    }

    /// <summary>Returns the stored access token, or null if not present.</summary>
    public string? GetAccessToken() => Session.GetString(AccessTokenKey);

    /// <summary>Returns the stored refresh token, or null if not present.</summary>
    public string? GetRefreshToken() => Session.GetString(RefreshTokenKey);

    /// <summary>Returns the stored user email, or null if not present.</summary>
    public string? GetUserEmail() => Session.GetString(UserEmailKey);

    /// <summary>Returns true when an access token exists in the session.</summary>
    public bool IsAuthenticated() => Session.GetString(AccessTokenKey) is not null;

    /// <summary>Clears all stored tokens and user info from the session.</summary>
    public void Clear()
    {
        Session.Remove(AccessTokenKey);
        Session.Remove(RefreshTokenKey);
        Session.Remove(UserEmailKey);
    }
}
