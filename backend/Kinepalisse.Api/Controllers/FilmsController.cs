using Kinepalisse.Api.Dtos;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/films")]
public class FilmsController : ControllerBase
{
    private readonly FilmService _service;
    public FilmsController(FilmService service) => _service = service;

    [HttpGet]
    public Task<IEnumerable<FilmListDto>> Lister() => _service.ListerAsync();

    [HttpGet("{id}")]
    public async Task<IActionResult> Recuperer(int id)
    {
        var film = await _service.RecupererAsync(id);
        return film == null ? NotFound() : Ok(film);
    }
}
