using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ApiLogin
{
    private readonly HttpClient _httpClientFactory;

    public ApiLogin(HttpClient httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // public async Task<DtoAuthentificationReponse> recupererToken(string login)
    // {
    //     var response = await _httpClientFactory.PostAsync($"/auth/login?login={login}",null);

    //     if (response.IsSuccessStatusCode)
    //     {
    //         var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(
    //             new JsonSerializerOptions(JsonSerializerDefaults.Web));

    //         if (string.IsNullOrWhiteSpace(tokenResponse?.Token))
    //         {
    //             return new DtoAuthentificationReponse(
    //                 (int)HttpStatusCode.InternalServerError,
    //                 null);
    //         }

    //         return new DtoAuthentificationReponse(200, tokenResponse.Token);
    //     }
    //     if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    //     {
    //         return new DtoAuthentificationReponse(401, null);
    //     }
    //     else
    //     {
    //         return new DtoAuthentificationReponse(((int)response.StatusCode), null);
    //     }
    // }

        public async Task<DtoAuthentificationReponse> recupererToken(string login, string password)
    {
        var loginPayload = new { Username = login, Password = password };
        var response = await _httpClientFactory.PostAsJsonAsync($"/auth/login", loginPayload);

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            {
                return new DtoAuthentificationReponse(
                    (int)HttpStatusCode.InternalServerError,
                    null);
            }

            return new DtoAuthentificationReponse(200, tokenResponse.AccessToken);
        }
        if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new DtoAuthentificationReponse(401, null);
        }
        else
        {
            return new DtoAuthentificationReponse(((int)response.StatusCode), null);
        }
    }

    // private sealed record TokenResponse(string Token);

    public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}

}
