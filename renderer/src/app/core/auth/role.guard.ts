import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

// Garde fonctionnel : retourne true si l'utilisateur a le rôle requis,
// sinon redirige vers la page de connexion.
export const roleGuard = (role: string): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return auth.hasRole(role) ? true : router.createUrlTree(['/auth/login']);
};
