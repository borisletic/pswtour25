import { Routes } from '@angular/router';

export const TOURS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./tour-list/tour-list.component').then(m => m.TourListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./tour-detail/tour-detail.component').then(m => m.TourDetailComponent)
  }
];