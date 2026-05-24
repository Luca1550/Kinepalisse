import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';

interface SeanceProcheDto {
  idSeance: number;
  dateHeure: string;
  filmTitre: string;
  nomSalle: string;
  prix: number;
}

@Component({
  selector: 'app-guichet',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './guichet.html',
  styleUrl: './guichet.css',
})
export class GuichetComponent {
  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  seances = toSignal(
    this.http.get<SeanceProcheDto[]>('/api/seances/proches'),
    { initialValue: [] }
  );

  seanceChoisie = signal<SeanceProcheDto | null>(null);
  recap         = signal<{ idReservation: number; montant: number } | null>(null);
  erreur        = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    nbPlaces:     [1, [Validators.required, Validators.min(1), Validators.max(10)]],
    modePaiement: ['Especes', Validators.required],
  });

  selectionner(s: SeanceProcheDto): void {
    this.seanceChoisie.set(s);
    this.recap.set(null);
    this.erreur.set(null);
    this.form.reset({ nbPlaces: 1, modePaiement: 'Especes' });
  }

  vendre(): void {
    const s = this.seanceChoisie();
    if (!s || this.form.invalid) return;
    this.erreur.set(null);
    this.http.post<{ idReservation: number; montant: number }>(
      '/api/reservations/guichet',
      { idSeance: s.idSeance, ...this.form.getRawValue() }
    ).subscribe({
      next: r => this.recap.set(r),
      error: err => this.erreur.set(err.error?.message ?? 'Erreur lors de la vente.'),
    });
  }
}
