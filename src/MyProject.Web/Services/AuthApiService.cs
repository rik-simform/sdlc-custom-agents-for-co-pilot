#nullable enable

using System.Net.Http.Json;
using System.Text.Json;

namespace MyProject.Web.Services;

/// <summary>
/// Typed HTTP client for the authentication API endpoints.
/// </summary>
public class AuthApiService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Registers a new user account.</summary>
    public async Task<(RegisterResponse? Data, string? Error)> RegisterAsync(
        RegisterRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/auth/register", request, ct);
        return await ParseResponseAsync<RegisterResponse>(response, ct);
    }

    /// <summary>Logs in and returns JWT tokens.</summary>
    public async Task<(LoginResponse? Data, string? Error)> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/auth/login", request, ct);
        return await ParseResponseAsync<LoginResponse>(response, ct);
    }

    /// <summary>Refreshes the access token using a refresh token.</summary>
    public async Task<(LoginResponse? Data, string? Error)> RefreshAsync(
        string refreshToken, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auth/refresh", new RefreshTokenRequest(refreshToken), ct);
        return await ParseResponseAsync<LoginResponse>(response, ct);
    }

    /// <summary>Revokes the refresh token (logout).</summary>
    public async Task<string?> RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auth/revoke", new RevokeTokenRequest(refreshToken), ct);
        if (response.IsSuccessStatusCode) return null;
        return await ExtractErrorAsync(response, ct);
    }

    private static async Task<(T? Data, string? Error)> ParseResponseAsync<T>(
        HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
            return (data, null);
        }
        var error = await ExtractErrorAsync(response, ct);
        return (default, error);
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync(ct);

            // Try to parse as JSON first
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var doc = JsonDocument.Parse(body);

                    // ASP.NET Core problem details format
                    if (doc.RootElement.TryGetProperty("detail", out var detail))
                        return detail.GetString() ?? response.ReasonPhrase ?? "Unknown error";

                    if (doc.RootElement.TryGetProperty("title", out var title))
                        return title.GetString() ?? response.ReasonPhrase ?? "Unknown error";

                    // ASP.NET Core validation errors
                    if (doc.RootElement.TryGetProperty("errors", out var errors))
                    {
                        var errorList = new List<string>();
                        foreach (var prop in errors.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var err1 in prop.Value.EnumerateArray())
                                {
                                    errorList.Add(err1.GetString() ?? "");
                                }
                            }
                        }
                        if (errorList.Count > 0)
                            return string.Join("; ", errorList);
                    }

                    // MediatR Result<T> error format (if your API returns this)
                    if (doc.RootElement.TryGetProperty("error", out var err))
                        return err.GetString() ?? response.ReasonPhrase ?? "Unknown error";
                }
                catch
                {
                    // Not JSON, return raw body
                    return $"{response.StatusCode}: {body}";
                }
            }

            return $"{response.StatusCode}: {response.ReasonPhrase ?? "Unknown error"}";
        }
        catch (Exception ex)
        {
            return $"{response.StatusCode}: {ex.Message}";
        }
    }
}
