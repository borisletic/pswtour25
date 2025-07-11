import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { PurchaseService } from './core/services/purchase.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatBadgeModule } from '@angular/material/badge';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    MatBadgeModule
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent implements OnInit {
  title = 'Tour App';
  isAuthenticated = false;
  userRole = '';
  cartItemCount = 0;

  constructor(
    private authService: AuthService,
    private purchaseService: PurchaseService
  ) {}

  ngOnInit(): void {
    // Subscribe to currentUser$ to track authentication changes
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
      if (user) {
        this.userRole = user.type;
        
        // Subscribe to cart changes for tourists
        if (this.userRole === 'Tourist') {
          this.purchaseService.cart$.subscribe(items => {
            this.cartItemCount = items.length;
          });
        }
      } else {
        this.userRole = '';
        this.cartItemCount = 0;
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}