import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
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
