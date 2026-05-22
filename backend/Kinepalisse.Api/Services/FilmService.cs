using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Dtos;
using MySqlConnector;

namespace Kinepalisse.Api.Services;

public class FilmService
{
    private readonly DbConnectionFactory _db;
    public FilmService(DbConnectionFactory db) => _db = db;

    // ----- LISTE -----
    public async Task<IEnumerable<FilmListDto>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        // QueryAsync<T> : retourne IEnumerable<T> en mappant chaque ligne sur T.
        // Les noms de colonnes (snake_case) sont mappés vers les propriétés C# (PascalCase)
        // grâce à `MatchNamesWithUnderscores = true` (Program.cs).
        return await conn.QueryAsync<FilmListDto>(@"
            SELECT id_film, titre, duree, affiche_url
            FROM Film
            ORDER BY titre");
    }

    // ----- DÉTAIL -----
    public async Task<FilmDetailDto?> RecupererAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        // 1. Film + réalisateur en une jointure
        var film = await conn.QuerySingleOrDefaultAsync<dynamic>(@"
            SELECT f.id_film, f.titre, f.duree, f.date_sortie, f.synopsis, f.affiche_url,
                   CONCAT_WS(' ', r.prenom, r.nom) AS realisateur
            FROM Film f
            LEFT JOIN Realisateur r ON r.id_realisateur = f.id_realisateur
            WHERE f.id_film = @id",
            new { id });
        if (film == null) return null;

        // 2. Les genres
        var genres = (await conn.QueryAsync<string>(@"
            SELECT g.nom_genre
            FROM Film_Genre fg
            JOIN Genre g ON g.id_genre = fg.id_genre
            WHERE fg.id_film = @id",
            new { id })).ToList();

        // 3. Les acteurs
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

    // ----- CRÉATION (film + liaisons N-N) -----
    public async Task<int> CreerAsync(CreateFilmDto dto)
    {
        using var conn = (MySqlConnection)await _db.CreateOpenConnectionAsync();

        // On ouvre une transaction : si la liaison des genres échoue,
        // on ne veut pas garder le film orphelin. → Atomicité ACID.
        using var tx = await conn.BeginTransactionAsync();
        try
        {
            var idFilm = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Film (titre, duree, date_sortie, synopsis, affiche_url, id_realisateur)
                VALUES (@Titre, @Duree, @DateSortie, @Synopsis, @AfficheUrl, @IdRealisateur);
                SELECT LAST_INSERT_ID();",
                dto, transaction: tx);

            if (dto.IdGenres.Count > 0)
                await conn.ExecuteAsync(
                    "INSERT INTO Film_Genre (id_film, id_genre) VALUES (@idFilm, @idGenre)",
                    dto.IdGenres.Select(g => new { idFilm, idGenre = g }),
                    transaction: tx);

            if (dto.IdActeurs.Count > 0)
                await conn.ExecuteAsync(
                    "INSERT INTO Film_Acteur (id_film, id_acteur) VALUES (@idFilm, @idActeur)",
                    dto.IdActeurs.Select(a => new { idFilm, idActeur = a }),
                    transaction: tx);

            await tx.CommitAsync();
            return idFilm;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // ----- SUPPRESSION (RG-08 : pas si séances futures) -----
    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        var infos = await conn.QuerySingleOrDefaultAsync<(int Existe, int SeancesFutures)>(@"
            SELECT
              (SELECT COUNT(*) FROM Film WHERE id_film = @id) AS Existe,
              (SELECT COUNT(*) FROM Seance WHERE id_film = @id AND date_heure > UTC_TIMESTAMP()) AS SeancesFutures",
            new { id });

        if (infos.Existe == 0) throw new Exception("Film introuvable.");
        if (infos.SeancesFutures > 0)
            throw new Exception("Impossible de supprimer : des séances futures existent.");

        // Les liaisons Film_Genre / Film_Acteur ont ON DELETE CASCADE en SQL → suppression auto.
        await conn.ExecuteAsync("DELETE FROM Film WHERE id_film = @id", new { id });
    }
}
