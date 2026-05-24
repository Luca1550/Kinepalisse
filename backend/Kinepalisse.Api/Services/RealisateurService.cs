using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;

namespace Kinepalisse.Api.Services;

public class RealisateurService
{
    private readonly DbConnectionFactory _db;
    public RealisateurService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Realisateur>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<Realisateur>(
            "SELECT id_realisateur, nom, prenom, date_naissance FROM Realisateur ORDER BY nom, prenom");
    }

    public async Task<int> CreerAsync(string nom, string prenom, DateOnly? dateNaissance)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Realisateur (nom, prenom, date_naissance) VALUES (@nom, @prenom, @dateNaissance);
            SELECT LAST_INSERT_ID();",
            new { nom, prenom, dateNaissance });
    }

    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        var films = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Film WHERE id_realisateur = @id", new { id });
        if (films > 0) throw new Exception("Réalisateur associé à des films existants.");
        await conn.ExecuteAsync("DELETE FROM Realisateur WHERE id_realisateur = @id", new { id });
    }
}
