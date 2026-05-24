using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/clients/me")]
[Authorize(Roles = "Client")]
public class ClientsMeController : ControllerBase
{
    private readonly ClientService _service;
    public ClientsMeController(ClientService s) => _service = s;

    private int IdUser => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? User.FindFirst("sub")!.Value);

    [HttpGet]
    public async Task<IActionResult> MonProfil()
    {
        var p = await _service.ProfilParUtilisateurAsync(IdUser);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPut]
    public async Task<IActionResult> MajProfil([FromBody] MajProfilDto dto)
    {
        await _service.MajProfilAsync(IdUser, dto.Nom, dto.Prenom, dto.Telephone);
        return NoContent();
    }

    [HttpGet("reservations")]
    public Task<IEnumerable<ClientService.MaResa>> MesReservations()
        => _service.ListerReservationsAsync(IdUser);
}

public record MajProfilDto(string Nom, string Prenom, string? Telephone);
