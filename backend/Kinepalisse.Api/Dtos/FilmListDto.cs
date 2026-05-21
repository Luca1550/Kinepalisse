namespace Kinepalisse.Api.Dtos;

public record FilmListDto(int IdFilm, string Titre, int Duree, string? AfficheUrl);

public record FilmDetailDto(
    int IdFilm,
    string Titre,
    int Duree,
    DateOnly? DateSortie,
    string? Synopsis,
    string? AfficheUrl,
    string? Realisateur,
    List<string> Genres,
    List<string> Acteurs
);
