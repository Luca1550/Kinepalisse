import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

// Garde fonctionnel : retourne true si l'utilisateur a le rôle requis,
// sinon redirige vers la page de connexion.
export const roleGuard = (...roles: string[]): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return roles.some(r => auth.hasRole(r)) ? true : router.createUrlTree(['/auth/login']);
};
