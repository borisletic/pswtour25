import { Routes } from '@angular/router';

export const PROFILE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./profile.component').then(m => m.ProfileComponent)
  },
  {
    path: 'purchases',
    loadComponent: () => import('./purchase-history/purchase-history.component').then(m => m.PurchaseHistoryComponent)
  },
  {
    path: '',
    redirectTo: 'purchases',
    pathMatch: 'full'
  }
];