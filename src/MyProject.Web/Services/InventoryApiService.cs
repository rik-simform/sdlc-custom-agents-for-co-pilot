#nullable enable

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyProject.Web.Services;

/// <summary>
/// Typed HTTP client for the inventory API endpoints.
/// </summary>
public class InventoryApiService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Sets the Bearer token for subsequent requests.</summary>
    public void SetBearerToken(string token) =>
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

    /// <summary>Returns all inventory items.</summary>
    public async Task<(IReadOnlyList<InventoryItemResponse>? Data, string? Error)> GetAllAsync(
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("/api/v1/inventory", ct);
        return await ParseListAsync(response, ct);
    }

    /// <summary>Returns items whose stock is below the reorder level.</summary>
    public async Task<(IReadOnlyList<InventoryItemResponse>? Data, string? Error)> GetLowStockAsync(
        CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("/api/v1/inventory/reorder", ct);
        return await ParseListAsync(response, ct);
    }

    /// <summary>Returns a single inventory item by id.</summary>
    public async Task<(InventoryItemResponse? Data, string? Error)> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"/api/v1/inventory/{id}", ct);
        return await ParseResponseAsync<InventoryItemResponse>(response, ct);
    }

    /// <summary>Creates a new inventory item.</summary>
    public async Task<(InventoryItemResponse? Data, string? Error)> CreateAsync(
        CreateInventoryItemRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/inventory", request, JsonOptions, ct);
        return await ParseResponseAsync<InventoryItemResponse>(response, ct);
    }

    /// <summary>Updates an existing inventory item.</summary>
    public async Task<(InventoryItemResponse? Data, string? Error)> UpdateAsync(
        Guid id, UpdateInventoryItemRequest request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/v1/inventory/{id}", request, JsonOptions, ct);
        return await ParseResponseAsync<InventoryItemResponse>(response, ct);
    }

    /// <summary>Deletes an inventory item.</summary>
    public async Task<string?> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/v1/inventory/{id}", ct);
        if (response.IsSuccessStatusCode) return null;
        return await ExtractErrorAsync(response, ct);
    }

    private static async Task<(IReadOnlyList<InventoryItemResponse>? Data, string? Error)> ParseListAsync(
        HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<InventoryItemResponse>>(JsonOptions, ct);
            return (data, null);
        }
        var error = await ExtractErrorAsync(response, ct);
        return (null, error);
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
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? response.ReasonPhrase ?? "Unknown error";
            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? response.ReasonPhrase ?? "Unknown error";
            return body;
        }
        catch
        {
            return response.ReasonPhrase ?? "Unknown error";
        }
    }
}
