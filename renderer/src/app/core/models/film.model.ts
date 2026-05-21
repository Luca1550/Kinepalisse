export interface FilmListItem {
  idFilm: number;
  titre: string;
  duree: number;
  afficheUrl: string | null;
}

export interface FilmDetail {
  idFilm: number;
  titre: string;
  duree: number;
  dateSortie: string | null;
  synopsis: string | null;
  afficheUrl: string | null;
  realisateur: string | null;
  genres: string[];
  acteurs: string[];
}
