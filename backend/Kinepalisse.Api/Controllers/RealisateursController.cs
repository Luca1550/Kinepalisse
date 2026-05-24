using Kinepalisse.Api.Models;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/realisateurs")]
public class RealisateursController : ControllerBase
{
    private readonly RealisateurService _service;
    public RealisateursController(RealisateurService s) => _service = s;

    [HttpGet]
    public Task<IEnumerable<Realisateur>> Lister() => _service.ListerAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerRealisateurDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto.Nom, dto.Prenom, dto.DateNaissance);
            return Ok(new { idRealisateur = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        try
        {
            await _service.SupprimerAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreerRealisateurDto(string Nom, string Prenom, DateOnly? DateNaissance);
