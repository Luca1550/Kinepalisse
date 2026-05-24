import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-films-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './films-admin.html',
})
export class FilmsAdminComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  films = signal<any[]>([]);
  realisateurs = toSignal(this.http.get<any[]>('/api/realisateurs'), { initialValue: [] });
  genres       = toSignal(this.http.get<any[]>('/api/genres'),       { initialValue: [] });
  acteurs      = toSignal(this.http.get<any[]>('/api/acteurs'),      { initialValue: [] });

  showForm  = signal(false);
  message   = signal<string | null>(null);
  erreur    = signal(false);

  genresChoisis  = signal<number[]>([]);
  acteursChoisis = signal<number[]>([]);

  form = this.fb.nonNullable.group({
    titre:          ['', Validators.required],
    duree:          [0,  [Validators.required, Validators.min(1)]],
    dateSortie:     [''],
    synopsis:       [''],
    afficheUrl:     [''],
    idRealisateur:  [0],
  });

  constructor() { this.charger(); }

  charger(): void {
    this.http.get<any[]>('/api/films').subscribe(data => this.films.set(data));
  }

  toggleForm(): void {
    this.showForm.update(v => !v);
    this.message.set(null);
  }

  onGenreChange(event: Event): void {
    const sel = event.target as HTMLSelectElement;
    this.genresChoisis.set(Array.from(sel.selectedOptions).map(o => Number(o.value)));
  }

  onActeurChange(event: Event): void {
    const sel = event.target as HTMLSelectElement;
    this.acteursChoisis.set(Array.from(sel.selectedOptions).map(o => Number(o.value)));
  }

  creer(): void {
    if (this.form.invalid) return;
    const raw = this.form.getRawValue();
    const payload = {
      titre:          raw.titre,
      duree:          Number(raw.duree),
      dateSortie:     raw.dateSortie || null,
      synopsis:       raw.synopsis   || null,
      afficheUrl:     raw.afficheUrl || null,
      idRealisateur:  raw.idRealisateur > 0 ? Number(raw.idRealisateur) : null,
      idGenres:       this.genresChoisis(),
      idActeurs:      this.acteursChoisis(),
    };
    this.http.post('/api/films', payload).subscribe({
      next: () => {
        this.erreur.set(false);
        this.message.set('Film créé.');
        this.form.reset({ titre: '', duree: 0, dateSortie: '', synopsis: '', afficheUrl: '', idRealisateur: 0 });
        this.genresChoisis.set([]);
        this.acteursChoisis.set([]);
        this.showForm.set(false);
        this.charger();
      },
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Erreur lors de la création.');
      }
    });
  }

  supprimer(id: number): void {
    this.http.delete(`/api/films/${id}`).subscribe({
      next: () => this.charger(),
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Impossible de supprimer.');
      }
    });
  }
}
