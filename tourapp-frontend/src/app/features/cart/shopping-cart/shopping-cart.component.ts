import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { PurchaseService } from '../../../core/services/purchase.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tour } from '../../../models/tour.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { firstValueFrom } from 'rxjs';
import { MatDivider } from "@angular/material/divider";


@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDivider
],
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit {
  cartItems: any[] = [];
  totalPrice = 0;
  bonusPointsAvailable = 0;
  bonusPointsToUse = 0;
  finalPrice = 0;
  loading = false;
  Math = Math;


  constructor(
    private purchaseService: PurchaseService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.purchaseService.cart$.subscribe(items => {
      this.cartItems = items;
      this.calculateTotals();
    });
    
    this.loadUserBonusPoints();

    this.authService.currentUser$.subscribe(user => {
      if (user && user.bonusPoints !== undefined) {
        this.bonusPointsAvailable = user.bonusPoints;
      }
    });
  }

  loadUserBonusPoints(): void {
    // Refresh user profile to get latest bonus points
    this.authService.refreshProfile().subscribe({
      next: (profile) => {
        this.bonusPointsAvailable = profile.bonusPoints || 0;
      },
      error: (error) => {
        console.error('Error loading bonus points:', error);
        this.bonusPointsAvailable = 0;
      }
    });
  }

  calculateTotals(): void {
    this.totalPrice = this.purchaseService.getTotalPrice();
    this.updateFinalPrice();
  }

  updateFinalPrice(): void {
    this.finalPrice = Math.max(0, this.totalPrice - this.bonusPointsToUse);
  }

  onBonusPointsChange(): void {
    if (this.bonusPointsToUse > this.bonusPointsAvailable) {
      this.bonusPointsToUse = this.bonusPointsAvailable;
    }
    if (this.bonusPointsToUse > this.totalPrice) {
      this.bonusPointsToUse = this.totalPrice;
    }
    this.updateFinalPrice();
  }

  removeFromCart(tourId: string): void {
    this.purchaseService.removeFromCart(tourId);
    this.snackBar.open('Tour removed from cart', 'Close', { duration: 2000 });
  }

   async checkout(): Promise<void> {
    if (this.cartItems.length === 0) {
      this.snackBar.open('Your cart is empty', 'Close', { duration: 3000 });
      return;
    }

    this.loading = true;
    
    try {
      const result = await firstValueFrom(this.purchaseService.purchase(this.bonusPointsToUse));
      this.purchaseService.clearCart();
      
      // Refresh user profile to update bonus points after purchase
      await firstValueFrom(this.authService.refreshProfile());
      
      this.snackBar.open('Purchase successful!', 'Close', { duration: 3000 });
      this.router.navigate(['/profile/purchases']);
    } catch (error: any) {
      console.error('Purchase error:', error);
      const errorMessage = error.error?.errors?.[0] || 'Purchase failed. Please try again.';
      this.snackBar.open(errorMessage, 'Close', { duration: 3000 });
    } finally {
      this.loading = false;
    }
  }
}