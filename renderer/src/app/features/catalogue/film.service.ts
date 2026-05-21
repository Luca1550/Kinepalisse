import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FilmListItem, FilmDetail } from '../../core/models/film.model';

@Injectable({ providedIn: 'root' })
export class FilmService {
  private http = inject(HttpClient);

  lister(): Observable<FilmListItem[]> {
    return this.http.get<FilmListItem[]>('/api/films');
  }

  recuperer(id: number): Observable<FilmDetail> {
    return this.http.get<FilmDetail>(`/api/films/${id}`);
  }
}
