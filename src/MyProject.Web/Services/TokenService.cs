#nullable enable

namespace MyProject.Web.Services;

/// <summary>
/// Stores and retrieves JWT tokens and user roles from the HTTP session.
/// </summary>
public class TokenService(IHttpContextAccessor httpContextAccessor)
{
    private const string AccessTokenKey = "AccessToken";
    private const string RefreshTokenKey = "RefreshToken";
    private const string UserEmailKey = "UserEmail";
    private const string UserRolesKey = "UserRoles";

    private ISession Session =>
        httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("No active HTTP session.");

    /// <summary>Saves the access token, refresh token, user email, and roles to session.</summary>
    public void StoreTokens(string accessToken, string refreshToken, string email, IList<string>? roles = null)
    {
        Session.SetString(AccessTokenKey, accessToken);
        Session.SetString(RefreshTokenKey, refreshToken);
        Session.SetString(UserEmailKey, email);
        
        if (roles?.Count > 0)
        {
            Session.SetString(UserRolesKey, string.Join(",", roles));
        }
    }

    /// <summary>Returns the stored access token, or null if not present.</summary>
    public string? GetAccessToken() => Session.GetString(AccessTokenKey);

    /// <summary>Returns the stored refresh token, or null if not present.</summary>
    public string? GetRefreshToken() => Session.GetString(RefreshTokenKey);

    /// <summary>Returns the stored user email, or null if not present.</summary>
    public string? GetUserEmail() => Session.GetString(UserEmailKey);

    /// <summary>Returns the user's roles as a list (comma-separated from session), or empty list if not present.</summary>
    public IList<string> GetUserRoles()
    {
        var rolesString = Session.GetString(UserRolesKey);
        return string.IsNullOrEmpty(rolesString)
            ? new List<string>()
            : rolesString.Split(',').ToList();
    }

    /// <summary>Returns true if the user has a specific role.</summary>
    public bool HasRole(string roleName)
    {
        var roles = GetUserRoles();
        return roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Returns true if the user has the Admin role.</summary>
    public bool IsAdmin() => HasRole("Admin");

    /// <summary>Returns true when an access token exists in the session.</summary>
    public bool IsAuthenticated() => Session.GetString(AccessTokenKey) is not null;

    /// <summary>Clears all stored tokens, roles, and user info from the session.</summary>
    public void Clear()
    {
        Session.Remove(AccessTokenKey);
        Session.Remove(RefreshTokenKey);
        Session.Remove(UserEmailKey);
        Session.Remove(UserRolesKey);
    }
}
