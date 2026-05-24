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
    path: 'admin',
    canActivate: [roleGuard('Admin')],
    loadComponent: () => import('./features/admin/admin-layout').then(m => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/admin/dashboard').then(m => m.DashboardComponent)
      },
      {
        path: 'films',
        loadComponent: () => import('./features/admin/films-admin').then(m => m.FilmsAdminComponent)
      },
      {
        path: 'acteurs',
        loadComponent: () => import('./features/admin/acteurs-admin').then(m => m.ActeursAdminComponent)
      },
      {
        path: 'realisateurs',
        loadComponent: () => import('./features/admin/realisateurs-admin').then(m => m.RealisateursAdminComponent)
      },
      {
        path: 'genres',
        loadComponent: () => import('./features/admin/genres-admin').then(m => m.GenresAdminComponent)
      },
      {
        path: 'salles',
        loadComponent: () => import('./features/admin/salles-admin').then(m => m.SallesAdminComponent)
      },
      {
        path: 'tarifs',
        loadComponent: () => import('./features/admin/tarifs-admin').then(m => m.TarifsAdminComponent)
      },
      {
        path: 'planning',
        loadComponent: () => import('./features/admin/planning').then(m => m.PlanningComponent)
      },
    ]
  },
  {
    path: 'reservation/:idSeance',
    canActivate: [() => inject(AuthService).user() != null
      ? true
      : inject(Router).createUrlTree(['/auth/login'])],
    loadComponent: () => import('./features/reservation/tunnel').then(m => m.TunnelComponent)
  },
  {
    path: 'guichet',
    canActivate: [roleGuard('Guichetier')],
    loadComponent: () => import('./features/guichet/guichet').then(m => m.GuichetComponent)
  },
  {
    path: 'compte',
    canActivate: [() => inject(AuthService).hasRole('Client') ? true : inject(Router).createUrlTree(['/auth/login'])],
    loadComponent: () => import('./features/compte/compte-layout').then(m => m.CompteLayoutComponent),
    children: [
      { path: '', redirectTo: 'profil', pathMatch: 'full' },
      {
        path: 'profil',
        loadComponent: () => import('./features/compte/profil').then(m => m.ProfilComponent)
      },
      {
        path: 'reservations',
        loadComponent: () => import('./features/compte/mes-reservations').then(m => m.MesReservationsComponent)
      },
    ]
  },
];
