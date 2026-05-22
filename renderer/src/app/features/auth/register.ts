import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <h1>Inscription</h1>
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <label>Nom
        <input type="text" formControlName="nom" />
      </label>
      <label>Prénom
        <input type="text" formControlName="prenom" />
      </label>
      <label>Email
        <input type="email" formControlName="email" />
      </label>
      <label>Mot de passe
        <input type="password" formControlName="motDePasse" />
      </label>
      <button type="submit" [disabled]="form.invalid">S'inscrire</button>
    </form>
    @if (erreur()) { <p style="color:red">{{ erreur() }}</p> }
    <p>Déjà un compte ? <a routerLink="/auth/login">Se connecter</a></p>
  `,
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.nonNullable.group({
    nom:        ['', Validators.required],
    prenom:     ['', Validators.required],
    email:      ['', [Validators.required, Validators.email]],
    motDePasse: ['', [Validators.required, Validators.minLength(6)]],
  });

  erreur = signal<string | null>(null);

  onSubmit(): void {
    if (this.form.invalid) return;
    const { nom, prenom, email, motDePasse } = this.form.getRawValue();
    // Inscription puis connexion automatique → redirige vers l'accueil
    this.auth.register(nom, prenom, email, motDePasse).subscribe({
      next: () => this.auth.login(email, motDePasse).subscribe({
        next: () => this.router.navigate(['/']),
        error: () => this.router.navigate(['/auth/login'])
      }),
      error: (err) => this.erreur.set(err?.error?.message ?? 'Erreur lors de l\'inscription.')
    });
  }
}
