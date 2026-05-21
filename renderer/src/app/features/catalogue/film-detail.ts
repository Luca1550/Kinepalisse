import { Component, input, inject, effect, signal } from '@angular/core';
import { FilmService } from './film.service';
import { FilmDetail } from '../../core/models/film.model';

@Component({
  selector: 'app-film-detail',
  template: `
    @if (film(); as f) {
      <h1>{{ f.titre }}</h1>
      <p><strong>Durée :</strong> {{ f.duree }} min</p>
      @if (f.realisateur) { <p><strong>Réalisateur :</strong> {{ f.realisateur }}</p> }
      @if (f.genres.length) { <p><strong>Genres :</strong> {{ f.genres.join(', ') }}</p> }
      @if (f.acteurs.length) { <p><strong>Avec :</strong> {{ f.acteurs.join(', ') }}</p> }
      @if (f.synopsis) { <p>{{ f.synopsis }}</p> }
    } @else {
      <p>Chargement…</p>
    }
  `,
})
export class FilmDetailComponent {
  id = input.required<string>();

  private filmService = inject(FilmService);
  film = signal<FilmDetail | null>(null);

  constructor() {
    effect(() => {
      const n = Number(this.id());
      this.filmService.recuperer(n).subscribe(f => this.film.set(f));
    });
  }
}
