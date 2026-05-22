import { Component, input, inject, signal, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

interface Dispo { capacite: number; dejaReserve: number; restant: number; }

@Component({
  selector: 'app-tunnel',
  imports: [ReactiveFormsModule, CurrencyPipe, RouterLink],
  templateUrl: './tunnel.html',
  styleUrl: './tunnel.css',
})
export class TunnelComponent {
  idSeance = input.required<string>();

  private http = inject(HttpClient);
  private fb   = inject(FormBuilder);

  etape    = signal(1);
  nbPlaces = signal(1);
  dispo    = signal<Dispo | null>(null);
  erreur   = signal<string | null>(null);
  confirmation = signal<{ idReservation: number; montant: number } | null>(null);

  peutSuivant = computed(() => {
    const d = this.dispo();
    return d != null && this.nbPlaces() > 0 && this.nbPlaces() <= d.restant;
  });

  cb = this.fb.nonNullable.group({
    numero: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],
    exp:    ['', [Validators.required, Validators.pattern(/^\d{2}\/\d{2}$/)]],
    cvv:    ['', [Validators.required, Validators.pattern(/^\d{3}$/)]],
  });

  constructor() {
    effect(() => {
      const n = Number(this.idSeance());
      this.http.get<Dispo>(`/api/seances/${n}/disponibilite`)
        .subscribe(d => this.dispo.set(d));
    });
  }

  setNbPlaces(event: Event): void {
    this.nbPlaces.set(+(event.target as HTMLInputElement).value);
  }

  payer(): void {
    if (this.cb.invalid) return;
    this.erreur.set(null);
    this.http.post<{ idReservation: number; montant: number }>('/api/reservations',
      { idSeance: Number(this.idSeance()), nbPlaces: this.nbPlaces() })
      .subscribe({
        next: r => { this.confirmation.set(r); this.etape.set(4); },
        error: e => this.erreur.set(e?.error?.message ?? 'Erreur lors de la réservation.'),
      });
  }
}
