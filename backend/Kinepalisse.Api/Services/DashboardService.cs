using Dapper;
using Kinepalisse.Api.Data;

namespace Kinepalisse.Api.Services;

public class DashboardService
{
    private readonly DbConnectionFactory _db;
    public DashboardService(DbConnectionFactory db) => _db = db;

    public record DashboardDto(int NbFilms, int NbSeancesAVenir, int NbReservationsJour, decimal TauxRemplissage);

    public async Task<DashboardDto> GetAsync()
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        return await conn.QuerySingleAsync<DashboardDto>(@"
            SELECT
              (SELECT COUNT(*) FROM Film) AS NbFilms,
              (SELECT COUNT(*) FROM Seance WHERE date_heure > UTC_TIMESTAMP()) AS NbSeancesAVenir,
              (SELECT COUNT(*) FROM Reservation
                 WHERE DATE(date_reservation) = DATE(UTC_TIMESTAMP())
                   AND statut = 'Confirmee') AS NbReservationsJour,
              COALESCE((
                SELECT ROUND(AVG(taux), 2) FROM (
                  SELECT SUM(r.nb_places) / sa.capacite AS taux
                  FROM Seance s
                  JOIN Salle sa ON sa.id_salle = s.id_salle
                  LEFT JOIN Reservation r
                    ON r.id_seance = s.id_seance AND r.statut = 'Confirmee'
                  WHERE s.date_heure > UTC_TIMESTAMP()
                  GROUP BY s.id_seance, sa.capacite
                ) AS sub
              ), 0) AS TauxRemplissage");
    }
}
