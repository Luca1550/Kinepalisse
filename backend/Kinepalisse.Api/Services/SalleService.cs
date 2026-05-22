using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;

namespace Kinepalisse.Api.Services;

public class SalleService
{
    private readonly DbConnectionFactory _db;
    public SalleService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Salle>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<Salle>(
            "SELECT id_salle, nom_salle, capacite FROM Salle ORDER BY nom_salle");
    }

    public async Task<int> CreerAsync(string nom, int capacite)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Salle (nom_salle, capacite) VALUES (@nom, @capacite);
            SELECT LAST_INSERT_ID();",
            new { nom, capacite });
    }

    // RG-09 : pas de suppression si séances futures
    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        var futures = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Seance WHERE id_salle = @id AND date_heure > UTC_TIMESTAMP()",
            new { id });
        if (futures > 0) throw new Exception("Salle utilisée par des séances futures.");
        await conn.ExecuteAsync("DELETE FROM Salle WHERE id_salle = @id", new { id });
    }
}
