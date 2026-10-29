using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthentificationApi.src.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("auth")]
public class AuthenticationController(IConfiguration configuration, IAuthentificationService authentificationService) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IAuthentificationService _authentificationService = authentificationService;

    /// <summary>
    /// on génère le token JWT à la connexion
    /// </summary>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Login( string login)
    {
        var userInfo = _authentificationService.getUserInfo(login).Result;

        if(userInfo == null)
        {
            return StatusCode(401, new { message = "Utilisateur non authentifié" });
        }

        var jwtKey = _configuration["Jwt:Key"] ?? "";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        return StatusCode(200, new { token = GenTokenJWT(key, userInfo.Role, userInfo.Login) });
    }

    /// <summary>
    /// Permet de générer un token JWT
    /// </summary>
    /// <param name="keyP"></param>
    /// <returns></returns>
    private string GenTokenJWT(byte[] keyP,string role, string login)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[] {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyP), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }
}