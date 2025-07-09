import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'malicious-users',
    loadComponent: () => import('./malicious-users/malicious-users.component').then(m => m.MaliciousUsersComponent)
  },
  {
    path: '',
    redirectTo: 'malicious-users',
    pathMatch: 'full'
  }
];