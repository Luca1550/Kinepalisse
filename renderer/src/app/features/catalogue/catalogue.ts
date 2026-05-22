import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { FilmService } from './film.service';

@Component({
  selector: 'app-catalogue',
  imports: [RouterLink],
  templateUrl: './catalogue.html',
  styleUrl: './catalogue.css',
})
export class CatalogueComponent {
  private filmService = inject(FilmService);
  films = toSignal(this.filmService.lister(), { initialValue: [] });
}
