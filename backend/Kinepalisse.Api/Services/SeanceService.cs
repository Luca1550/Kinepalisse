using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Dtos;
using MySqlConnector;

namespace Kinepalisse.Api.Services;

public class SeanceService
{
    private readonly DbConnectionFactory _db;
    private static readonly TimeSpan NETTOYAGE = TimeSpan.FromMinutes(30);

    public SeanceService(DbConnectionFactory db) => _db = db;

    private record SeanceAvecDuree(int IdSeance, DateTime DateHeure, int Duree);
    private record DispoRow(int Capacite, decimal DejaReserve);

    public async Task<int> PlanifierAsync(int idFilm, int idSalle, int idTarif, DateTime dateHeure)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        // 1. Durée du film à planifier
        var duree = await conn.QuerySingleOrDefaultAsync<int?>(
            "SELECT duree FROM Film WHERE id_film = @idFilm",
            new { idFilm });
        if (duree == null) throw new Exception("Film introuvable.");

        // 2. Fenêtre occupée par la NOUVELLE séance
        DateTime debutNouvelle = dateHeure;
        DateTime finNouvelle   = debutNouvelle.AddMinutes(duree.Value).Add(NETTOYAGE);

        // 3. Séances existantes dans cette salle, autour du créneau (filtre J±1).
        //    L'index (id_salle, date_heure) de Seance accélère cette requête.
        var existantes = await conn.QueryAsync<SeanceAvecDuree>(@"
            SELECT s.id_seance AS IdSeance, s.date_heure AS DateHeure, f.duree AS Duree
            FROM Seance s
            JOIN Film f ON f.id_film = s.id_film
            WHERE s.id_salle = @idSalle
              AND s.date_heure > @borneInf
              AND s.date_heure < @borneSup",
            new
            {
                idSalle,
                borneInf = debutNouvelle.AddDays(-1),
                borneSup = finNouvelle.AddDays(1)
            });

        // 4. Formule du chevauchement : A < D ET C < B
        foreach (var s in existantes)
        {
            DateTime debutS = s.DateHeure;
            DateTime finS   = s.DateHeure.AddMinutes(s.Duree).Add(NETTOYAGE);

            if (debutNouvelle < finS && debutS < finNouvelle)
                throw new Exception("La salle est déjà occupée sur ce créneau.");
        }

        // 5. Pas de conflit → on enregistre
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO Seance (date_heure, id_film, id_salle, id_tarif)
            VALUES (@dateHeure, @idFilm, @idSalle, @idTarif);
            SELECT LAST_INSERT_ID();",
            new { dateHeure, idFilm, idSalle, idTarif });
    }

    public async Task<object> DisponibiliteAsync(int idSeance)
    {
        using var conn = await _db.CreateOpenConnectionAsync();

        var row = await conn.QuerySingleOrDefaultAsync<DispoRow>(@"
            SELECT sa.capacite AS Capacite,
                   COALESCE((SELECT SUM(r.nb_places)
                             FROM Reservation r
                             WHERE r.id_seance = s.id_seance
                               AND r.statut = 'Confirmee'), 0) AS DejaReserve
            FROM Seance s
            JOIN Salle sa ON sa.id_salle = s.id_salle
            WHERE s.id_seance = @idSeance",
            new { idSeance });

        if (row == null) throw new Exception("Séance introuvable.");

        return new
        {
            capacite    = row.Capacite,
            dejaReserve = row.DejaReserve,
            restant     = row.Capacite - row.DejaReserve
        };
    }

    public async Task<IEnumerable<SeanceProcheDto>> ListerProchesAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<SeanceProcheDto>(@"
            SELECT s.id_seance AS IdSeance, s.date_heure AS DateHeure,
                   f.titre AS FilmTitre, sa.nom_salle AS NomSalle, t.prix AS Prix
            FROM Seance s
            JOIN Film f  ON f.id_film  = s.id_film
            JOIN Salle sa ON sa.id_salle = s.id_salle
            JOIN Tarif t  ON t.id_tarif  = s.id_tarif
            WHERE s.date_heure > UTC_TIMESTAMP()
              AND s.date_heure < DATE_ADD(UTC_TIMESTAMP(), INTERVAL 24 HOUR)
            ORDER BY s.date_heure");
    }

    public async Task<IEnumerable<SeanceDuFilmDto>> ListerParFilmAsync(int idFilm)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QueryAsync<SeanceDuFilmDto>(@"
            SELECT s.id_seance AS IdSeance, s.date_heure AS DateHeure,
                   sa.nom_salle AS NomSalle, sa.id_salle AS IdSalle,
                   t.type_tarif AS TypeTarif, t.prix AS Prix
            FROM Seance s
            JOIN Salle sa ON sa.id_salle = s.id_salle
            JOIN Tarif t  ON t.id_tarif  = s.id_tarif
            WHERE s.id_film = @idFilm AND s.date_heure > UTC_TIMESTAMP()
            ORDER BY s.date_heure",
            new { idFilm });
    }
}
