namespace Kinepalisse.Api.Dtos;

public record SeanceDuFilmDto(
    int IdSeance,
    DateTime DateHeure,
    string NomSalle,
    int IdSalle,
    string TypeTarif,
    decimal Prix
);
