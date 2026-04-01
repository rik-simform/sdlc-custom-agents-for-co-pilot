#nullable enable

namespace MyProject.Infrastructure.Services;

/// <summary>
/// Configuration options for JWT token generation.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the signing key (from Key Vault in production).
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access token expiry in minutes. Default: 15.
    /// </summary>
    public int AccessTokenExpiryMinutes { get; set; } = 15;
}
