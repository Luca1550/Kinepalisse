import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-profil',
  imports: [ReactiveFormsModule],
  templateUrl: './profil.html',
  styleUrl: './profil.css',
})
export class ProfilComponent {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);

  form: FormGroup | null = null;
  ok = signal(false);

  constructor() {
    this.http.get<any>('/api/clients/me').subscribe(p => {
      this.form = this.fb.nonNullable.group({
        nom:       [p.nom,             Validators.required],
        prenom:    [p.prenom,          Validators.required],
        telephone: [p.telephone ?? ''],
      });
    });
  }

  save(): void {
    if (!this.form || this.form.invalid) return;
    this.http.put('/api/clients/me', this.form.getRawValue()).subscribe(() => {
      this.ok.set(true);
      setTimeout(() => this.ok.set(false), 2000);
    });
  }
}
