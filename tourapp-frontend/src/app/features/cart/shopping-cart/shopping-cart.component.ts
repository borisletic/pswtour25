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

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    //RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
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
  }

  loadUserBonusPoints(): void {
    // In real app, this would be loaded from user profile
    // For now, we'll assume it's part of the user data
    const user = this.authService.getCurrentUser();
    this.bonusPointsAvailable = user.bonusPoints || 0;
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
      const result = await this.purchaseService.purchase(this.bonusPointsToUse).toPromise();
      this.purchaseService.clearCart();
      this.snackBar.open('Purchase successful!', 'Close', { duration: 3000 });
      this.router.navigate(['/profile/purchases']);
    } catch (error) {
      this.snackBar.open('Purchase failed. Please try again.', 'Close', { duration: 3000 });
    } finally {
      this.loading = false;
    }
  }
}