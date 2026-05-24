using Dapper;
using Kinepalisse.Api.Data;

namespace Kinepalisse.Api.Services;

public class ClientService
{
    private readonly DbConnectionFactory _db;
    public ClientService(DbConnectionFactory db) => _db = db;

    public record ProfilDto(int IdClient, string Nom, string Prenom, string? Email, string? Telephone);

    public async Task<ProfilDto?> ProfilParUtilisateurAsync(int idUser)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QuerySingleOrDefaultAsync<ProfilDto>(@"
            SELECT id_client AS IdClient, nom AS Nom, prenom AS Prenom,
                   email AS Email, telephone AS Telephone
            FROM Client WHERE id_utilisateur = @idUser",
            new { idUser });
    }

    public async Task MajProfilAsync(int idUser, string nom, string prenom, string? telephone)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        await conn.ExecuteAsync(@"
            UPDATE Client SET nom = @nom, prenom = @prenom, telephone = @telephone
            WHERE id_utilisateur = @idUser",
            new { idUser, nom, prenom, telephone });
    }

    public record MaResa(int IdReservation, int IdSeance, DateTime DateHeure,
                         string FilmTitre, int NbPlaces, string Statut);

    public async Task<IEnumerable<MaResa>> ListerReservationsAsync(int idUser)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<MaResa>(@"
            SELECT r.id_reservation AS IdReservation, r.id_seance AS IdSeance,
                   s.date_heure AS DateHeure, f.titre AS FilmTitre,
                   r.nb_places AS NbPlaces, r.statut AS Statut
            FROM Reservation r
            JOIN Client  c ON c.id_client = r.id_client
            JOIN Seance  s ON s.id_seance = r.id_seance
            JOIN Film    f ON f.id_film   = s.id_film
            WHERE c.id_utilisateur = @idUser
            ORDER BY s.date_heure DESC",
            new { idUser });
    }
}
