#if DEBUG
using Dapper;
using Kinepalisse.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/dev")]
public class DevController : ControllerBase
{
    private readonly DbConnectionFactory _db;
    public DevController(DbConnectionFactory db) => _db = db;

    [HttpPost("seed-users")]
    public async Task<IActionResult> SeedUsers()
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        async Task EnsureUser(string email, string role)
        {
            var existe = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Utilisateur WHERE email = @email", new { email });
            if (existe > 0) return;
            var hash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 11);
            await conn.ExecuteAsync(@"
                INSERT INTO Utilisateur (email, mot_de_passe_hash, role, date_creation)
                VALUES (@email, @hash, @role, UTC_TIMESTAMP())",
                new { email, hash, role });
        }

        await EnsureUser("admin@kine.be",   "Admin");
        await EnsureUser("guichet@kine.be", "Guichetier");

        // Client générique utilisé pour les ventes au guichet (pas de compte utilisateur)
        var existeGen = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Client WHERE nom = 'Guichet' AND prenom = 'Generique'");
        if (existeGen == 0)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO Client (nom, prenom, email, id_utilisateur)
                VALUES ('Guichet', 'Generique', NULL, NULL)");
        }

        return Ok(new { admin   = "admin@kine.be / admin123",
                        guichet = "guichet@kine.be / admin123",
                        clientGenerique = "créé si absent" });
    }
}
#endif
