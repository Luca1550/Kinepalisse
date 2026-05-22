import { Routes } from '@angular/router';
import { roleGuard } from './core/auth/role.guard';

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
];
