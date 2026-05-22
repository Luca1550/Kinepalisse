using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/tarifs")]
public class TarifsController : ControllerBase
{
    private readonly TarifService _service;
    public TarifsController(TarifService s) => _service = s;

    [HttpGet]
    public Task<IEnumerable<Kinepalisse.Api.Models.Tarif>> Lister() => _service.ListerAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerTarifDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto.TypeTarif, dto.Prix);
            return Ok(new { idTarif = id });
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

public record CreerTarifDto(string TypeTarif, decimal Prix);
