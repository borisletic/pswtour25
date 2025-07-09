import { Routes } from '@angular/router';

export const CART_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./shopping-cart/shopping-cart.component').then(m => m.ShoppingCartComponent)
  }
];