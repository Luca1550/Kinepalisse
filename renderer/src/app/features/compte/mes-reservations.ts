import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';

export interface MaResa {
  idReservation: number;
  idSeance: number;
  dateHeure: string;
  filmTitre: string;
  nbPlaces: number;
  statut: string;
}

@Component({
  selector: 'app-mes-reservations',
  imports: [DatePipe],
  templateUrl: './mes-reservations.html',
  styleUrl: './mes-reservations.css',
})
export class MesReservationsComponent {
  private http = inject(HttpClient);

  resas = signal<MaResa[]>([]);

  constructor() {
    this.http.get<MaResa[]>('/api/clients/me/reservations').subscribe(r => this.resas.set(r));
  }

  peutAnnuler(r: MaResa): boolean {
    if (r.statut !== 'Confirmee') return false;
    const delaiMs = new Date(r.dateHeure).getTime() - Date.now();
    return delaiMs > 2 * 60 * 60 * 1000;
  }

  annuler(id: number): void {
    if (!confirm('Confirmer l\'annulation ?')) return;
    this.http.delete(`/api/reservations/${id}`).subscribe({
      next: () => this.resas.update(arr =>
        arr.map(r => r.idReservation === id ? { ...r, statut: 'Annulee' } : r)),
      error: e => alert(e?.error?.message ?? 'Erreur lors de l\'annulation.'),
    });
  }
}
