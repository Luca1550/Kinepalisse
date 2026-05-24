using Dapper;
using Kinepalisse.Api.Data;
using MySqlConnector;

namespace Kinepalisse.Api.Services;

public class ReservationService
{
    private readonly DbConnectionFactory _db;
    public ReservationService(DbConnectionFactory db) => _db = db;

    private record SeanceInfo(DateTime DateHeure, int Capacite, decimal Prix);

    public async Task<(int idReservation, decimal montant)> ReserverAsync(
        int idClient, int idSeance, int nbPlaces)
    {
        using var conn = (MySqlConnection)await _db.CreateOpenConnectionAsync();
        return await ReserverInterneAsync(conn, idClient, idSeance, nbPlaces, "CarteEnLigne");
    }

    public async Task<(int idReservation, decimal montant)> ReserverGuichetAsync(
        int idSeance, int nbPlaces, string modePaiement)
    {
        if (modePaiement != "Especes" && modePaiement != "Bancontact")
            throw new Exception("Mode de paiement invalide.");

        using var conn = (MySqlConnection)await _db.CreateOpenConnectionAsync();

        var idClient = await conn.ExecuteScalarAsync<int?>(
            "SELECT id_client FROM Client WHERE nom = 'Guichet' AND prenom = 'Generique'");
        if (idClient == null)
            throw new Exception("Client générique introuvable. Lance POST /api/dev/seed-users.");

        return await ReserverInterneAsync(conn, idClient.Value, idSeance, nbPlaces, modePaiement);
    }

    private async Task<(int idReservation, decimal montant)> ReserverInterneAsync(
        MySqlConnection conn, int idClient, int idSeance, int nbPlaces, string modePaiement)
    {
        // 1. Récupérer infos séance + tarif + capacité en UNE requête
        var info = await conn.QuerySingleOrDefaultAsync<SeanceInfo>(@"
            SELECT s.date_heure AS DateHeure,
                   sa.capacite  AS Capacite,
                   t.prix       AS Prix
            FROM Seance s
            JOIN Salle sa ON sa.id_salle = s.id_salle
            JOIN Tarif t  ON t.id_tarif  = s.id_tarif
            WHERE s.id_seance = @idSeance",
            new { idSeance });
        if (info == null) throw new Exception("Séance introuvable.");

        // 2. RG-05 : séance pas déjà passée
        if (info.DateHeure <= DateTime.UtcNow)
            throw new Exception("Cette séance a déjà commencé.");

        // 3. RG-04 : capacité disponible ?
        var dejaReserve = await conn.ExecuteScalarAsync<int>(@"
            SELECT COALESCE(SUM(nb_places), 0)
            FROM Reservation
            WHERE id_seance = @idSeance AND statut = 'Confirmee'",
            new { idSeance });
        if (dejaReserve + nbPlaces > info.Capacite)
            throw new Exception("Places insuffisantes.");

        // 4. RG-10 : montant calculé côté serveur, jamais depuis le client
        decimal montant = nbPlaces * info.Prix;

        // 5. Transaction : INSERT Reservation + INSERT Paiement → tout ou rien (Atomicité ACID)
        using var tx = await conn.BeginTransactionAsync();
        try
        {
            var idResa = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Reservation (id_client, id_seance, nb_places, date_reservation, statut)
                VALUES (@idClient, @idSeance, @nbPlaces, UTC_TIMESTAMP(), 'Confirmee');
                SELECT LAST_INSERT_ID();",
                new { idClient, idSeance, nbPlaces },
                transaction: tx);

            await conn.ExecuteAsync(@"
                INSERT INTO Paiement (id_reservation, montant, mode_paiement, statut, date_paiement)
                VALUES (@idResa, @montant, @modePaiement, 'Paye', UTC_TIMESTAMP());",
                new { idResa, montant, modePaiement },
                transaction: tx);

            await tx.CommitAsync();
            return (idResa, montant);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private record ResaInfo(int IdUtilisateur, DateTime DateHeureSeance);

    public async Task AnnulerAsync(int idReservation, int idUserCourant, string roleCourant)
    {
        using var conn = (MySqlConnection)await _db.CreateOpenConnectionAsync();

        // 1. Récupérer propriétaire + date de la séance en une requête
        var info = await conn.QuerySingleOrDefaultAsync<ResaInfo>(@"
            SELECT c.id_utilisateur AS IdUtilisateur,
                   s.date_heure      AS DateHeureSeance
            FROM Reservation r
            JOIN Client c ON c.id_client = r.id_client
            JOIN Seance s ON s.id_seance = r.id_seance
            WHERE r.id_reservation = @idReservation",
            new { idReservation });
        if (info == null) throw new Exception("Réservation introuvable.");

        // 2. RG-07 : un client ne peut annuler que les siennes (Admin/Guichetier passent)
        bool estProprietaire = info.IdUtilisateur == idUserCourant;
        if (!estProprietaire && roleCourant != "Admin" && roleCourant != "Guichetier")
            throw new Exception("Accès refusé.");

        // 3. RG-06 : règle des 2 h (uniquement pour le client propriétaire)
        if (estProprietaire && roleCourant == "Client")
        {
            var delai = info.DateHeureSeance - DateTime.UtcNow;
            if (delai <= TimeSpan.FromHours(2))
                throw new Exception("Annulation impossible à moins de 2h.");
        }

        // 4. Soft delete + remboursement paiement (transaction)
        using var tx = await conn.BeginTransactionAsync();
        try
        {
            await conn.ExecuteAsync(
                "UPDATE Reservation SET statut = 'Annulee' WHERE id_reservation = @idReservation",
                new { idReservation }, transaction: tx);

            await conn.ExecuteAsync(
                "UPDATE Paiement SET statut = 'Rembourse' WHERE id_reservation = @idReservation",
                new { idReservation }, transaction: tx);

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // Depuis l'id Utilisateur extrait du JWT, on retrouve l'idClient puis on délègue
    public async Task<(int idReservation, decimal montant)> ReserverPourUtilisateurAsync(
        int idUser, int idSeance, int nbPlaces)
    {
        using var conn = await _db.CreateOpenConnectionAsync();
        var idClient = await conn.ExecuteScalarAsync<int?>(
            "SELECT id_client FROM Client WHERE id_utilisateur = @idUser",
            new { idUser });
        if (idClient == null) throw new Exception("Compte client introuvable.");
        return await ReserverAsync(idClient.Value, idSeance, nbPlaces);
    }
}
