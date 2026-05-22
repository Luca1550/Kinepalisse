import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <h1>Connexion</h1>
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <label>Email
        <input type="email" formControlName="email" />
      </label>
      <label>Mot de passe
        <input type="password" formControlName="motDePasse" />
      </label>
      <button type="submit" [disabled]="form.invalid">Se connecter</button>
    </form>
    @if (erreur()) { <p style="color:red">{{ erreur() }}</p> }
    <p>Pas de compte ? <a routerLink="/auth/register">S'inscrire</a></p>
  `,
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.nonNullable.group({
    email:      ['', [Validators.required, Validators.email]],
    motDePasse: ['', [Validators.required, Validators.minLength(6)]],
  });

  erreur = signal<string | null>(null);

  onSubmit(): void {
    if (this.form.invalid) return;
    const { email, motDePasse } = this.form.getRawValue();
    this.auth.login(email, motDePasse).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => this.erreur.set('Identifiants invalides.')
    });
  }
}
