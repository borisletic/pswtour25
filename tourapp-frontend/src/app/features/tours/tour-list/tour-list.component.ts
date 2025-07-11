import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TourService } from '../../../core/services/tour.service';
import { PurchaseService } from '../../../core/services/purchase.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tour, TourFilter, Interest, TourDifficulty } from '../../../models/tour.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-tour-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatSnackBarModule
  ],
  templateUrl: './tour-list.component.html',
  styleUrls: ['./tour-list.component.scss']
})
export class TourListComponent implements OnInit, OnDestroy {
  tours: Tour[] = [];
  filteredTours: Tour[] = [];
  loading = false;
  filter: TourFilter = {};
  
  interests = [
    { value: Interest.Nature, label: 'Nature' },
    { value: Interest.Art, label: 'Art' },
    { value: Interest.Sport, label: 'Sport' },
    { value: Interest.Shopping, label: 'Shopping' },
    { value: Interest.Food, label: 'Food' }
  ];
  
  difficulties = [
    { value: TourDifficulty.Easy, label: 'Easy' },
    { value: TourDifficulty.Medium, label: 'Medium' },
    { value: TourDifficulty.Hard, label: 'Hard' },
    { value: TourDifficulty.Expert, label: 'Expert' }
  ];
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private tourService: TourService,
    private purchaseService: PurchaseService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit(): void {
    this.loadTours();
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  loadTours(): void {
    this.loading = true;
    // Don't send empty filter object
    const hasFilters = this.filter && Object.keys(this.filter).length > 0;
    
    this.tourService.getTours(hasFilters ? this.filter : undefined)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (tours) => {
          this.tours = tours;
          this.filteredTours = tours;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading tours:', error);
          this.snackBar.open('Failed to load tours', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
  }
  
  applyFilters(): void {
    this.loadTours();
  }
  
  clearFilters(): void {
    this.filter = {};
    this.loadTours();
  }
  
  addToCart(tour: Tour): void {
    if (!this.authService.isAuthenticated()) {
      this.snackBar.open('Please login to add tours to cart', 'Close', { duration: 3000 });
      return;
    }
    
    if (!this.authService.hasRole('Tourist')) {
      this.snackBar.open('Only tourists can purchase tours', 'Close', { duration: 3000 });
      return;
    }
    
    this.purchaseService.addToCart(tour);
    this.snackBar.open('Tour added to cart', 'Close', { duration: 2000 });
  }
  
  isTourist(): boolean {
    return this.authService.hasRole('Tourist');
  }
}