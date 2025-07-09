import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/tours',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'tours',
    loadChildren: () => import('./features/tours/tours.routes').then(m => m.TOURS_ROUTES)
  },
  {
    path: 'cart',
    loadChildren: () => import('./features/cart/cart.routes').then(m => m.CART_ROUTES),
    canActivate: [authGuard],
    data: { role: 'Tourist' }
  },
  {
    path: 'guide',
    loadChildren: () => import('./features/guide/guide.routes').then(m => m.GUIDE_ROUTES),
    canActivate: [authGuard],
    data: { role: 'Guide' }
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES),
    canActivate: [authGuard],
    data: { role: 'Administrator' }
  },
  {
    path: 'problems',
    loadChildren: () => import('./features/problems/problems.routes').then(m => m.PROBLEMS_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    loadChildren: () => import('./features/profile/profile.routes').then(m => m.PROFILE_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'unauthorized',
    loadComponent: () => import('./features/errors/unauthorized.component').then(m => m.UnauthorizedComponent)
  },
  {
    path: '**',
    redirectTo: '/tours'
  }
];