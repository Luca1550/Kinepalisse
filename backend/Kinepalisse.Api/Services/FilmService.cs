using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Dtos;

namespace Kinepalisse.Api.Services;

public class FilmService
{
    private readonly DbConnectionFactory _db;
    public FilmService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<FilmListDto>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<FilmListDto>(@"
            SELECT id_film, titre, duree, affiche_url
            FROM Film
            ORDER BY titre");
    }

    public async Task<FilmDetailDto?> RecupererAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        var film = await conn.QuerySingleOrDefaultAsync<dynamic>(@"
            SELECT f.id_film, f.titre, f.duree, f.date_sortie, f.synopsis, f.affiche_url,
                   CONCAT_WS(' ', r.prenom, r.nom) AS realisateur
            FROM Film f
            LEFT JOIN Realisateur r ON r.id_realisateur = f.id_realisateur
            WHERE f.id_film = @id",
            new { id });
        if (film == null) return null;

        var genres = (await conn.QueryAsync<string>(@"
            SELECT g.nom_genre
            FROM Film_Genre fg
            JOIN Genre g ON g.id_genre = fg.id_genre
            WHERE fg.id_film = @id",
            new { id })).ToList();

        var acteurs = (await conn.QueryAsync<string>(@"
            SELECT CONCAT_WS(' ', a.prenom, a.nom)
            FROM Film_Acteur fa
            JOIN Acteur a ON a.id_acteur = fa.id_acteur
            WHERE fa.id_film = @id",
            new { id })).ToList();

        return new FilmDetailDto(
            (int)film.id_film,
            (string)film.titre,
            (int)film.duree,
            film.date_sortie == null ? null : DateOnly.FromDateTime((DateTime)film.date_sortie),
            film.synopsis as string,
            film.affiche_url as string,
            film.realisateur as string,
            genres,
            acteurs
        );
    }
}
