namespace Kinepalisse.Api.Dtos;

public record SeanceProcheDto(
    int IdSeance,
    DateTime DateHeure,
    string FilmTitre,
    string NomSalle,
    decimal Prix
);
