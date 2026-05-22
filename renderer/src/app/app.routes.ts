import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { roleGuard } from './core/auth/role.guard';
import { AuthService } from './core/auth/auth.service';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/catalogue/catalogue').then(m => m.CatalogueComponent)
  },
  {
    path: 'films/:id',
    loadComponent: () => import('./features/catalogue/film-detail').then(m => m.FilmDetailComponent)
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/login').then(m => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/auth/register').then(m => m.RegisterComponent)
  },
  {
    path: 'admin/planning',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./features/admin/planning').then(m => m.PlanningComponent)
  },
  {
    path: 'reservation/:idSeance',
    canActivate: [() => inject(AuthService).user() != null
      ? true
      : inject(Router).createUrlTree(['/auth/login'])],
    loadComponent: () => import('./features/reservation/tunnel').then(m => m.TunnelComponent)
  },
];
