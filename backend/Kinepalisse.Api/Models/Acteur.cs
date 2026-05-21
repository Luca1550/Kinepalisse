namespace Kinepalisse.Api.Models;

public class Acteur
{
    public int IdActeur { get; set; }
    public string Nom { get; set; } = "";
    public string Prenom { get; set; } = "";
    public DateOnly? DateNaissance { get; set; }
}
