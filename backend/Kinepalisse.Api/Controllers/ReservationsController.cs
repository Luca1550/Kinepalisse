using System.ComponentModel.DataAnnotations;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _service;
    public ReservationsController(ReservationService s) => _service = s;

    [Authorize(Roles = "Client")]
    [HttpPost]
    public async Task<IActionResult> Reserver([FromBody] ReserverDto dto)
    {
        try
        {
            // L'idUtilisateur vient du JWT, jamais du body — sinon un client pourrait
            // poster idClient = 42 pour réserver au nom de quelqu'un d'autre.
            var idUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? User.FindFirst("sub")!.Value);

            var (idRes, montant) = await _service.ReserverPourUtilisateurAsync(idUser, dto.IdSeance, dto.NbPlaces);
            return Ok(new { idReservation = idRes, montant });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Guichetier,Admin")]
    [HttpPost("guichet")]
    public async Task<IActionResult> ReserverGuichet([FromBody] ReserverGuichetDto dto)
    {
        try
        {
            var (id, montant) = await _service.ReserverGuichetAsync(dto.IdSeance, dto.NbPlaces, dto.ModePaiement);
            return Ok(new { idReservation = id, montant });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Annuler(int id)
    {
        try
        {
            var idUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? User.FindFirst("sub")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            await _service.AnnulerAsync(id, idUser, role);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record ReserverDto(int IdSeance, [Range(1, 10)] int NbPlaces);
public record ReserverGuichetDto(int IdSeance, [Range(1, 10)] int NbPlaces, string ModePaiement);
