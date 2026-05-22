import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-planning',
  imports: [ReactiveFormsModule],
  templateUrl: './planning.html',
})
export class PlanningComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  films  = toSignal(this.http.get<any[]>('/api/films'),  { initialValue: [] });
  salles = toSignal(this.http.get<any[]>('/api/salles'), { initialValue: [] });
  tarifs = toSignal(this.http.get<any[]>('/api/tarifs'), { initialValue: [] });

  form = this.fb.nonNullable.group({
    idFilm:    [0, [Validators.required, Validators.min(1)]],
    idSalle:   [0, [Validators.required, Validators.min(1)]],
    idTarif:   [0, [Validators.required, Validators.min(1)]],
    dateHeure: ['', Validators.required],
  });

  message = signal<string | null>(null);
  erreur  = signal(false);

  onSubmit(): void {
    if (this.form.invalid) return;
    const raw = this.form.getRawValue();
    const payload = {
      idFilm:    Number(raw.idFilm),
      idSalle:   Number(raw.idSalle),
      idTarif:   Number(raw.idTarif),
      dateHeure: new Date(raw.dateHeure).toISOString(),
    };
    this.http.post<{ idSeance: number }>('/api/seances', payload).subscribe({
      next: r => {
        this.erreur.set(false);
        this.message.set(`Séance n°${r.idSeance} créée avec succès.`);
        this.form.reset({ idFilm: 0, idSalle: 0, idTarif: 0, dateHeure: '' });
      },
      error: e => {
        this.erreur.set(true);
        this.message.set(e?.error?.message ?? 'Erreur lors de la planification.');
      }
    });
  }
}
