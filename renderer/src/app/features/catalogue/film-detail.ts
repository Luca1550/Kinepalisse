import { Component, input, inject, effect, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FilmService } from './film.service';
import { FilmDetail } from '../../core/models/film.model';

interface SeanceDuFilm {
  idSeance: number;
  dateHeure: string;
  nomSalle: string;
  idSalle: number;
  typeTarif: string;
  prix: number;
}

@Component({
  selector: 'app-film-detail',
  imports: [DatePipe, RouterLink],
  templateUrl: './film-detail.html',
})
export class FilmDetailComponent {
  id = input.required<string>();

  private filmService = inject(FilmService);
  private http = inject(HttpClient);
  film    = signal<FilmDetail | null>(null);
  seances = signal<SeanceDuFilm[]>([]);

  constructor() {
    effect(() => {
      const n = Number(this.id());
      this.filmService.recuperer(n).subscribe(f => this.film.set(f));
      this.http.get<SeanceDuFilm[]>(`/api/films/${n}/seances`)
        .subscribe(s => this.seances.set(s));
    });
  }
}
