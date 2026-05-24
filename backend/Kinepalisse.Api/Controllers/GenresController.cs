using Kinepalisse.Api.Models;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly GenreService _service;
    public GenresController(GenreService s) => _service = s;

    [HttpGet]
    public Task<IEnumerable<Genre>> Lister() => _service.ListerAsync();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerGenreDto dto)
    {
        try
        {
            var id = await _service.CreerAsync(dto.NomGenre);
            return Ok(new { idGenre = id });
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

public record CreerGenreDto(string NomGenre);
