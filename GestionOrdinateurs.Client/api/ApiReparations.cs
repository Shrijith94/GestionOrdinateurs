using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.SessionStorage;

public class ApiReparations
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;

    public ApiReparations(HttpClient httpClient, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _sessionStorage = sessionStorage;
    }

    public async Task<List<DtoReparationHistorique>> ObtenirHistoriqueAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/reparation/historique");
        var token = await _sessionStorage.GetItemAsync<string>("token");
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<DtoReparationHistorique>>() ?? [];
    }
}