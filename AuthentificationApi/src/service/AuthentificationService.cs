namespace AuthentificationApi.src.service;
public class AuthentificationService : IAuthentificationService{
    HttpClient _httpClient;

    public AuthentificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserInfo?> getUserInfo(string login)
    {
        var response = await _httpClient.GetAsync("/api/users/login/"+login);
        if (response.IsSuccessStatusCode)
        {
            
            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            return userInfo;
        }
        else
        {
            return null;
        }
    }
}