import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-tarifs-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './tarifs-admin.html',
})
export class TarifsAdminComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  tarifs   = signal<any[]>([]);
  showForm = signal(false);
  message  = signal<string | null>(null);
  erreur   = signal(false);

  form = this.fb.nonNullable.group({
    typeTarif: ['', Validators.required],
    prix:      [0,  [Validators.required, Validators.min(0)]],
  });

  constructor() { this.charger(); }

  charger(): void {
    this.http.get<any[]>('/api/tarifs').subscribe(data => this.tarifs.set(data));
  }

  toggleForm(): void {
    this.showForm.update(v => !v);
    this.message.set(null);
  }

  creer(): void {
    if (this.form.invalid) return;
    this.http.post('/api/tarifs', this.form.getRawValue()).subscribe({
      next: () => {
        this.erreur.set(false);
        this.message.set('Tarif créé.');
        this.form.reset({ typeTarif: '', prix: 0 });
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
    this.http.delete(`/api/tarifs/${id}`).subscribe({
      next: () => this.charger(),
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Impossible de supprimer.');
      }
    });
  }
}
