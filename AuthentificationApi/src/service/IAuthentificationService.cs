namespace AuthentificationApi.src.service;

public interface IAuthentificationService
{
    Task<UserInfo?> getUserInfo(string login);
}