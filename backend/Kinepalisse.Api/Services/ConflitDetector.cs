namespace Kinepalisse.Api.Services;

public static class ConflitDetector
{
    public static readonly TimeSpan NETTOYAGE = TimeSpan.FromMinutes(30);

    public record CreneauExistant(DateTime DateHeure, int Duree);

    public static bool ExisteConflit(
        DateTime nouvelleDate, int nouvelleDuree,
        IEnumerable<CreneauExistant> existantes)
    {
        DateTime debutNouvelle = nouvelleDate;
        DateTime finNouvelle   = debutNouvelle.AddMinutes(nouvelleDuree).Add(NETTOYAGE);

        foreach (var s in existantes)
        {
            DateTime debutS = s.DateHeure;
            DateTime finS   = s.DateHeure.AddMinutes(s.Duree).Add(NETTOYAGE);
            if (debutNouvelle < finS && debutS < finNouvelle) return true;
        }
        return false;
    }
}
