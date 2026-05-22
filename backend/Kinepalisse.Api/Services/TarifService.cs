using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;

namespace Kinepalisse.Api.Services;

public class TarifService
{
    private readonly DbConnectionFactory _db;
    public TarifService(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Tarif>> ListerAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<Tarif>(
            "SELECT id_tarif, type_tarif, prix FROM Tarif ORDER BY prix");
    }

    public async Task<int> CreerAsync(string typeTarif, decimal prix)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Tarif (type_tarif, prix) VALUES (@typeTarif, @prix);
            SELECT LAST_INSERT_ID();",
            new { typeTarif, prix });
    }

    public async Task SupprimerAsync(int id)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        var futures = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Seance WHERE id_tarif = @id AND date_heure > UTC_TIMESTAMP()",
            new { id });
        if (futures > 0) throw new Exception("Tarif utilisé par des séances futures.");
        await conn.ExecuteAsync("DELETE FROM Tarif WHERE id_tarif = @id", new { id });
    }
}
