import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
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
