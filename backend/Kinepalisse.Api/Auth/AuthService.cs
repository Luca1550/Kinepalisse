using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kinepalisse.Api.Auth;

public class AuthService
{
    private readonly DbConnectionFactory _db;
    private readonly IConfiguration _config;

    public AuthService(DbConnectionFactory db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // INSCRIPTION (crée un Utilisateur "Client" + le Client lié)
    public async Task<int> RegisterAsync(string nom, string prenom, string email, string motDePasse)
    {
        using var conn = (MySqlConnector.MySqlConnection)await _db.CreateOpenConnectionAsync();

        // 1. Email déjà pris ?
        var existe = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Utilisateur WHERE email = @email",
            new { email });
        if (existe > 0) throw new Exception("Email déjà utilisé.");

        // 2. Hacher (BCrypt gère le salt automatiquement)
        var hash = BCrypt.Net.BCrypt.HashPassword(motDePasse, workFactor: 11);

        // 3. Transaction : Utilisateur + Client ensemble ou pas du tout
        using var tx = await conn.BeginTransactionAsync();
        try
        {
            var idUser = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Utilisateur (email, mot_de_passe_hash, role, date_creation)
                VALUES (@email, @hash, 'Client', UTC_TIMESTAMP());
                SELECT LAST_INSERT_ID();",
                new { email, hash }, transaction: tx);

            var idClient = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Client (nom, prenom, email, id_utilisateur)
                VALUES (@nom, @prenom, @email, @idUser);
                SELECT LAST_INSERT_ID();",
                new { nom, prenom, email, idUser }, transaction: tx);

            await tx.CommitAsync();
            return idClient;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // CONNEXION (renvoie le JWT en cas de succès)
    public async Task<string> LoginAsync(string email, string motDePasse)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        var user = await conn.QuerySingleOrDefaultAsync<Utilisateur>(@"
            SELECT id_utilisateur, email, mot_de_passe_hash, role, date_creation
            FROM Utilisateur
            WHERE email = @email",
            new { email });
        if (user == null) throw new Exception("Identifiants invalides.");

        if (!BCrypt.Net.BCrypt.Verify(motDePasse, user.MotDePasseHash))
            throw new Exception("Identifiants invalides.");

        return GenererJwt(user);
    }

    private string GenererJwt(Utilisateur user)
    {
        // a) Les "claims" = ce qu'on met dans le token (sub = identifiant standard JWT)
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.IdUtilisateur.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // b) Clé de signature symétrique (lue depuis appsettings) — même clé pour signer et vérifier
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // c) Construire le token
        var token = new JwtSecurityToken(
            issuer:    _config["Jwt:Issuer"],
            audience:  _config["Jwt:Audience"],
            claims:    claims,
            expires:   DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        // d) Sérialiser en chaîne base64url (les 3 parties séparées par des points)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
