using Kinepalisse.Api.Dtos;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/films")]
public class FilmsController : ControllerBase
{
    private readonly FilmService _service;
    private readonly SeanceService _seances;
    public FilmsController(FilmService service, SeanceService seances)
    {
        _service = service;
        _seances = seances;
    }

    [HttpGet]
    public Task<IEnumerable<FilmListDto>> Lister() => _service.ListerAsync();

    [HttpGet("{id}")]
    public async Task<IActionResult> Recuperer(int id)
    {
        var film = await _service.RecupererAsync(id);
        return film == null ? NotFound() : Ok(film);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreateFilmDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto);
            return CreatedAtAction(nameof(Recuperer), new { id }, new { idFilm = id });
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

    [HttpGet("{id}/seances")]
    public Task<IEnumerable<SeanceDuFilmDto>> SeancesDuFilm(int id)
        => _seances.ListerParFilmAsync(id);
}
