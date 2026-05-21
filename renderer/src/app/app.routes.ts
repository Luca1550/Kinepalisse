import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/catalogue/catalogue').then(m => m.CatalogueComponent)
  },
  {
    path: 'films/:id',
    loadComponent: () => import('./features/catalogue/film-detail').then(m => m.FilmDetailComponent)
  },
];
