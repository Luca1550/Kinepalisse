using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;

namespace Kinepalisse.Api.Services;

public class ActeurService
{
    private readonly DbConnectionFactory _db;
    public ActeurService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Acteur>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<Acteur>(
            "SELECT id_acteur, nom, prenom, date_naissance FROM Acteur ORDER BY nom, prenom");
    }

    public async Task<int> CreerAsync(string nom, string prenom, DateOnly? dateNaissance)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Acteur (nom, prenom, date_naissance) VALUES (@nom, @prenom, @dateNaissance);
            SELECT LAST_INSERT_ID();",
            new { nom, prenom, dateNaissance });
    }

    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        // ON DELETE CASCADE sur Film_Acteur → libère les liaisons automatiquement
        await conn.ExecuteAsync("DELETE FROM Acteur WHERE id_acteur = @id", new { id });
    }
}
