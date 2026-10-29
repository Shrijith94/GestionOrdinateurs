using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.SessionStorage;

public class ApiOrdinateurs
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorage;

    public ApiOrdinateurs(HttpClient httpClient, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _sessionStorage = sessionStorage;
    }

    public async Task<List<DtoOrdinateur>> ObtenirTousLesOrdinateursAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/ordinateurs");
        await AjouterTokenAsync(request);

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var resultat = await response.Content.ReadFromJsonAsync<DtoOrdinateursResponse>();
        return resultat?.Ordinateurs ?? [];
    }

    private async Task AjouterTokenAsync(HttpRequestMessage request)
    {
        var token = await _sessionStorage.GetItemAsync<string>("token");
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}