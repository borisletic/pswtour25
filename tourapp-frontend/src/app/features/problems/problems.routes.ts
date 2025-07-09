import { Routes } from '@angular/router';

export const PROBLEMS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./problem-management/problem-management.component').then(m => m.ProblemManagementComponent)
  }
];