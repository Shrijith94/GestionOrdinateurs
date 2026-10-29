namespace GestionOrdinateurs.API.src.controller;
using GestionOrdinateurs.Shared.src.model;
using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.Shared.src.enums;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService ordinateurService, ILogger<UsersController> logger)
    {
        _userService = ordinateurService;
        _logger = logger;
    }
    
    [HttpGet("login/{login}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenirOrdinateur(string login)
    {
        var UtilisateurExistant = await _userService.ObtenirUtilisateursParLogin(login);
        if (UtilisateurExistant == null)
        {
            _logger.LogWarning("GET - api/user/login/"+ login +" - utilisateur non trouvé");
            return StatusCode(404, new { message = "Utilisateur non trouvé" });
        }
        _logger.LogInformation("GET - api/user/login/"+ login +" - Récupération de l'utilisateur avec le login " + login);
        return Ok( UtilisateurExistant );
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ObtenirTousLesUtilisateurs()
    {
        var utilisateurs = await _userService.ObtenirTousLesUtilisateurs();
        return Ok(utilisateurs);
    }
}