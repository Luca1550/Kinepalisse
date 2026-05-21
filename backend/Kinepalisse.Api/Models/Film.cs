namespace Kinepalisse.Api.Models;

public class Film
{
    public int IdFilm { get; set; }
    public string Titre { get; set; } = "";
    public int Duree { get; set; }
    public DateOnly? DateSortie { get; set; }
    public string? Synopsis { get; set; }
    public string? AfficheUrl { get; set; }
    public int? IdRealisateur { get; set; }
}
