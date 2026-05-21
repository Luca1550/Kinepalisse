namespace Kinepalisse.Api.Models;

public class Realisateur
{
    public int IdRealisateur { get; set; }
    public string Nom { get; set; } = "";
    public string Prenom { get; set; } = "";
    public DateOnly? DateNaissance { get; set; }
}
