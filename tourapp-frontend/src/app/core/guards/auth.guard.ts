import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard = (route: any) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const requiredRole = route.data?.['role'];
    
    if (requiredRole && !authService.hasRole(requiredRole)) {
      router.navigate(['/unauthorized']);
      return false;
    }
    
    return true;
  }

  router.navigate(['/auth/login'], { queryParams: { returnUrl: route.url } });
  return false;
};