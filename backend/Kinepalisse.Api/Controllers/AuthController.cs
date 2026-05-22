using Kinepalisse.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var id = await _auth.RegisterAsync(dto.Nom, dto.Prenom, dto.Email, dto.MotDePasse);
            return Ok(new { idClient = id });
        }
        catch (Exception ex)
        {
            // 400 Bad Request avec le message métier (ex : "Email déjà utilisé.")
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _auth.LoginAsync(dto.Email, dto.MotDePasse);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            // 401 Unauthorized : identifiants incorrects
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        id    = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? User.FindFirst("sub")?.Value,
        email = User.FindFirst(ClaimTypes.Email)?.Value,
        role  = User.FindFirst(ClaimTypes.Role)?.Value
    });
}

public record RegisterDto(string Nom, string Prenom, string Email, string MotDePasse);
public record LoginDto(string Email, string MotDePasse);
