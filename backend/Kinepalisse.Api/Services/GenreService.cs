using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;

namespace Kinepalisse.Api.Services;

public class GenreService
{
    private readonly DbConnectionFactory _db;
    public GenreService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Genre>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<Genre>(
            "SELECT id_genre, nom_genre FROM Genre ORDER BY nom_genre");
    }

    public async Task<int> CreerAsync(string nomGenre)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Genre (nom_genre) VALUES (@nomGenre);
            SELECT LAST_INSERT_ID();",
            new { nomGenre });
    }

    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        var films = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Film_Genre WHERE id_genre = @id", new { id });
        if (films > 0) throw new Exception("Genre associé à des films existants.");
        await conn.ExecuteAsync("DELETE FROM Genre WHERE id_genre = @id", new { id });
    }
}
