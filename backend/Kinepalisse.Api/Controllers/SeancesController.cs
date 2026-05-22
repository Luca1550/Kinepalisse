using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/seances")]
public class SeancesController : ControllerBase
{
    private readonly SeanceService _service;
    public SeancesController(SeanceService s) => _service = s;

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Planifier([FromBody] PlanifierDto dto)
    {
        try
        {
            var id = await _service.PlanifierAsync(dto.IdFilm, dto.IdSalle, dto.IdTarif, dto.DateHeure);
            return Ok(new { idSeance = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/disponibilite")]
    public async Task<IActionResult> Disponibilite(int id)
    {
        try
        {
            var dispo = await _service.DisponibiliteAsync(id);
            return Ok(dispo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record PlanifierDto(int IdFilm, int IdSalle, int IdTarif, DateTime DateHeure);
