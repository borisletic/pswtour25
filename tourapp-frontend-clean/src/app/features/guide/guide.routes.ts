import { Routes } from '@angular/router';

export const GUIDE_ROUTES: Routes = [
  {
    path: 'tours',
    loadComponent: () => import('./my-tours/my-tours.component').then(m => m.MyToursComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./create-tour/create-tour.component').then(m => m.CreateTourComponent)
  },
  {
    path: '',
    redirectTo: 'tours',
    pathMatch: 'full'
  }
];