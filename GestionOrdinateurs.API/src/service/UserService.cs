using GestionOrdinateurs.API.src.data;
namespace GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.Shared.src.model;
using GestionOrdinateurs.API.src.data;
using GestionOrdinateurs.Shared.src.enums;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.API.src.events;
using GestionOrdinateurs.API.src.abonnee;
using Microsoft.EntityFrameworkCore;
public class UserService
{
    private readonly ParcInformatiqueContext _context ;

    public UserService(ParcInformatiqueContext parcInformatiqueContextParam)
    {
        _context = parcInformatiqueContextParam;
    }

    private static  DtoUserResponse ConvertirUserEnDto(User user)
    {
        return new DtoUserResponse(
            user.Id,
            user.Login,
            user.Role.ToString()
        );
    }

    public async Task<DtoUserResponse?> ObtenirUtilisateursParLogin(string login)
    {
        var user = await _context.Users.Where(user => user.Login == login)
        .Select(o => ConvertirUserEnDto(o))
        .FirstOrDefaultAsync();
        return user;
    }

    public async Task<List<DtoUserResponse>> ObtenirTousLesUtilisateurs()
    {
        var users = await _context.Users
        .Select(o => ConvertirUserEnDto(o))
        .ToListAsync();
        return users;
    }
}