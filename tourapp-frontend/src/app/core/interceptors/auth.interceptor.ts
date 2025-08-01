import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  console.log('Auth interceptor called for:', req.url);
  
  
  const router = inject(Router);
  
  const token = localStorage.getItem('auth_token');
  console.log('Token found:', !!token);
  
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    console.log('Added Authorization header');
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('HTTP Error:', error);
      if (error.status === 401) {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('user_data');
        router.navigate(['/auth/login']);
      }
      return throwError(() => error);
    })
  );
};