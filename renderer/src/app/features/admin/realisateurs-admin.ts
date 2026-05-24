import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-realisateurs-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './realisateurs-admin.html',
})
export class RealisateursAdminComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  realisateurs = signal<any[]>([]);
  showForm     = signal(false);
  message      = signal<string | null>(null);
  erreur       = signal(false);

  form = this.fb.nonNullable.group({
    nom:           ['', Validators.required],
    prenom:        ['', Validators.required],
    dateNaissance: [''],
  });

  constructor() { this.charger(); }

  charger(): void {
    this.http.get<any[]>('/api/realisateurs').subscribe(data => this.realisateurs.set(data));
  }

  toggleForm(): void {
    this.showForm.update(v => !v);
    this.message.set(null);
  }

  creer(): void {
    if (this.form.invalid) return;
    const raw = this.form.getRawValue();
    this.http.post('/api/realisateurs', {
      nom:           raw.nom,
      prenom:        raw.prenom,
      dateNaissance: raw.dateNaissance || null,
    }).subscribe({
      next: () => {
        this.erreur.set(false);
        this.message.set('Réalisateur créé.');
        this.form.reset({ nom: '', prenom: '', dateNaissance: '' });
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
    this.http.delete(`/api/realisateurs/${id}`).subscribe({
      next: () => this.charger(),
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Impossible de supprimer.');
      }
    });
  }
}
