import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-genres-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './genres-admin.html',
})
export class GenresAdminComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  genres   = signal<any[]>([]);
  showForm = signal(false);
  message  = signal<string | null>(null);
  erreur   = signal(false);

  form = this.fb.nonNullable.group({
    nomGenre: ['', Validators.required],
  });

  constructor() { this.charger(); }

  charger(): void {
    this.http.get<any[]>('/api/genres').subscribe(data => this.genres.set(data));
  }

  toggleForm(): void {
    this.showForm.update(v => !v);
    this.message.set(null);
  }

  creer(): void {
    if (this.form.invalid) return;
    this.http.post('/api/genres', this.form.getRawValue()).subscribe({
      next: () => {
        this.erreur.set(false);
        this.message.set('Genre créé.');
        this.form.reset({ nomGenre: '' });
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
    this.http.delete(`/api/genres/${id}`).subscribe({
      next: () => this.charger(),
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Impossible de supprimer.');
      }
    });
  }
}
