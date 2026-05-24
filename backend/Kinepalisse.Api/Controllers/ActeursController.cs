using Kinepalisse.Api.Models;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/acteurs")]
public class ActeursController : ControllerBase
{
    private readonly ActeurService _service;
    public ActeursController(ActeurService s) => _service = s;

    [HttpGet]
    public Task<IEnumerable<Acteur>> Lister() => _service.ListerAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerActeurDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto.Nom, dto.Prenom, dto.DateNaissance);
            return Ok(new { idActeur = id });
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

public record CreerActeurDto(string Nom, string Prenom, DateOnly? DateNaissance);
