using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/salles")]
public class SallesController : ControllerBase
{
    private readonly SalleService _service;
    public SallesController(SalleService s) => _service = s;

    [HttpGet]
    public Task<IEnumerable<Kinepalisse.Api.Models.Salle>> Lister() => _service.ListerAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerSalleDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto.Nom, dto.Capacite);
            return Ok(new { idSalle = id });
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

public record CreerSalleDto(string Nom, int Capacite);
