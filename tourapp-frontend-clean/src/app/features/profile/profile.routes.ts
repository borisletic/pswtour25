import { Routes } from '@angular/router';

export const PROFILE_ROUTES: Routes = [
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