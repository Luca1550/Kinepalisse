import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { FilmService } from './film.service';

@Component({
  selector: 'app-catalogue',
  imports: [RouterLink],
  template: `
    <h1>Catalogue</h1>
    <div class="grille">
      @for (film of films(); track film.idFilm) {
        <a class="carte" [routerLink]="['/films', film.idFilm]">
          @if (film.afficheUrl) {
            <img [src]="film.afficheUrl" [alt]="film.titre" />
          }
          <h3>{{ film.titre }}</h3>
          <p>{{ film.duree }} min</p>
        </a>
      } @empty {
        <p>Aucun film disponible pour le moment.</p>
      }
    </div>
  `,
  styles: [`
    .grille { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 1rem; }
    .carte { border: 1px solid #ccc; padding: .5rem; text-decoration: none; color: inherit; display: block; }
  `]
})
export class CatalogueComponent {
  private filmService = inject(FilmService);
  films = toSignal(this.filmService.lister(), { initialValue: [] });
}
