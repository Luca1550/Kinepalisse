import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-salles-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './salles-admin.html',
})
export class SallesAdminComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  salles   = signal<any[]>([]);
  showForm = signal(false);
  message  = signal<string | null>(null);
  erreur   = signal(false);

  form = this.fb.nonNullable.group({
    nom:      ['', Validators.required],
    capacite: [0,  [Validators.required, Validators.min(1)]],
  });

  constructor() { this.charger(); }

  charger(): void {
    this.http.get<any[]>('/api/salles').subscribe(data => this.salles.set(data));
  }

  toggleForm(): void {
    this.showForm.update(v => !v);
    this.message.set(null);
  }

  creer(): void {
    if (this.form.invalid) return;
    this.http.post('/api/salles', this.form.getRawValue()).subscribe({
      next: () => {
        this.erreur.set(false);
        this.message.set('Salle créée.');
        this.form.reset({ nom: '', capacite: 0 });
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
    this.http.delete(`/api/salles/${id}`).subscribe({
      next: () => this.charger(),
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Impossible de supprimer.');
      }
    });
  }
}
