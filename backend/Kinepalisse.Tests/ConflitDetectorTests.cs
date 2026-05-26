using Kinepalisse.Api.Services;

public class ConflitDetectorTests
{
    [Fact]
    public void Chevauchement_RetourneTrue()
    {
        var existantes = new[]
        {
            new ConflitDetector.CreneauExistant(
                new DateTime(2026, 6, 25, 18, 0, 0, DateTimeKind.Utc), 90)
        };
        bool conflit = ConflitDetector.ExisteConflit(
            new DateTime(2026, 6, 25, 19, 0, 0, DateTimeKind.Utc), 90, existantes);
        Assert.True(conflit);
    }

    [Fact]
    public void DebutEgalFinPlusNettoyage_PasDeConflit()
    {
        var existantes = new[]
        {
            new ConflitDetector.CreneauExistant(
                new DateTime(2026, 6, 25, 18, 0, 0, DateTimeKind.Utc), 90)
        };
        // 18:00 + 90min + 30min nettoyage = 20:00 → démarrer à 20:00 pile est autorisé
        bool conflit = ConflitDetector.ExisteConflit(
            new DateTime(2026, 6, 25, 20, 0, 0, DateTimeKind.Utc), 90, existantes);
        Assert.False(conflit);
    }

    [Theory]
    [InlineData(17, 30, 60, true)]  // 17:30 + 60min + 30min = 19:00, chevauche 18:00–19:30
    [InlineData(22,  0, 90, false)] // 22:00, bien après la fin → pas de conflit
    public void Cas_Multiples(int h, int m, int duree, bool attendu)
    {
        var existantes = new[]
        {
            new ConflitDetector.CreneauExistant(
                new DateTime(2026, 6, 25, 18, 0, 0, DateTimeKind.Utc), 90)
        };
        bool conflit = ConflitDetector.ExisteConflit(
            new DateTime(2026, 6, 25, h, m, 0, DateTimeKind.Utc), duree, existantes);
        Assert.Equal(attendu, conflit);
    }
}
