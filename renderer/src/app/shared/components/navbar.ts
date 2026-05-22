import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink],
  template: `
    <nav>
      <a routerLink="/">Catalogue</a>
      @if (auth.user(); as u) {
        <span>Connecté : {{ u.email }} ({{ u.role }})</span>
        <button (click)="auth.logout()">Déconnexion</button>
      } @else {
        <a routerLink="/auth/login">Connexion</a>
        <a routerLink="/auth/register">S'inscrire</a>
      }
    </nav>
  `,
})
export class NavbarComponent {
  auth = inject(AuthService);
}

