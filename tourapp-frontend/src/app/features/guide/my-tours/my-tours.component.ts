import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TourService } from '../../../core/services/tour.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tour, TourStatus } from '../../../models/tour.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-my-tours',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    MatTooltipModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './my-tours.component.html',
  styleUrls: ['./my-tours.component.scss']
})
export class MyToursComponent implements OnInit {
  tours: Tour[] = [];
  filteredTours: Tour[] = [];
  loading = false;
  filterStatus: 'all' | 'draft' | 'published' = 'all';
  
  displayedColumns = ['name', 'category', 'difficulty', 'scheduledDate', 'status', 'ratings', 'actions'];

  constructor(
    private tourService: TourService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadMyTours();
  }

  private safeToLowerCase(value: any): string {
    try {
      if (value === null || value === undefined) {
        return '';
      }
      return String(value).toLowerCase();
    } catch (error) {
      console.error('Error in safeToLowerCase:', error, value);
      return '';
    }
  }

  private isValidTour(tour: any): boolean {
    try {
      return tour && 
             typeof tour === 'object' && 
             tour.hasOwnProperty('status') &&
             tour.status !== null &&
             tour.status !== undefined;
    } catch (error) {
      console.error('Error checking tour validity:', error);
      return false;
    }
  }

  loadMyTours(): void {
    this.loading = true;
    
    try {
      const user = this.authService.getCurrentUser();
      
      if (!user || !user.id) {
        this.snackBar.open('User not found', 'Close', { duration: 3000 });
        this.loading = false;
        return;
      }
      
      this.tourService.getTours({ guideId: user.id }).subscribe({
        next: (tours) => {
          try {
            console.log('Raw tours data:', tours);
            
            // Ensure tours is an array and has valid data
            if (!Array.isArray(tours)) {
              console.error('Tours is not an array:', tours);
              this.tours = [];
            } else {
              this.tours = tours;
            }
            
            // Log status values for debugging
            this.tours.forEach((tour, index) => {
              console.log(`Tour ${index}:`, {
                name: tour?.name || 'Unknown',
                status: tour?.status,
                statusType: typeof tour?.status,
                wholeObject: tour
              });
            });
            
            this.applyFilter();
            this.loading = false;
          } catch (error) {
            console.error('Error processing tours data:', error);
            this.tours = [];
            this.filteredTours = [];
            this.loading = false;
          }
        },
        error: (error) => {
          console.error('Error loading tours:', error);
          this.snackBar.open('Failed to load tours', 'Close', { duration: 3000 });
          this.tours = [];
          this.filteredTours = [];
          this.loading = false;
        }
      });
    } catch (error) {
      console.error('Error in loadMyTours:', error);
      this.loading = false;
    }
  }

  onFilterChange(newFilter: any): void {
  console.log('Filter changed to:', newFilter);
  this.filterStatus = newFilter;
  this.applyFilter();
}

  applyFilter(): void {
  console.log('=== APPLYING FILTER ===');
  console.log('Current filter:', this.filterStatus);
  console.log('Available tours:', this.tours.length);
  
  if (!Array.isArray(this.tours)) {
    console.error('Tours is not an array:', this.tours);
    this.filteredTours = [];
    return;
  }
  
  if (this.filterStatus === 'all') {
    this.filteredTours = [...this.tours];
    console.log('Filter = ALL, showing all tours:', this.filteredTours.length);
  } else if (this.filterStatus === 'draft' || this.filterStatus === 'published') {
    // Direct status comparison - check multiple variations
    this.filteredTours = this.tours.filter(tour => {
      if (!tour || !tour.status) {
        console.log('Tour has no status:', tour);
        return false;
      }
      
      const tourStatus = String(tour.status);
      const targetStatus = this.filterStatus === 'draft' ? 'Draft' : 'Published';
      
      console.log(`Checking tour "${tour.name}": "${tourStatus}" === "${targetStatus}"`);
      
      // Check exact match with proper capitalization
      const isMatch = tourStatus === targetStatus;
      
      console.log(`Match result: ${isMatch}`);
      return isMatch;
    });
    
    console.log(`Filter = ${this.filterStatus.toUpperCase()}, found ${this.filteredTours.length} tours`);
  } else {
    console.log('Unknown filter status:', this.filterStatus);
    this.filteredTours = [...this.tours]; // Show all if unknown filter
  }
  
  console.log('Final filtered tours:', this.filteredTours.map(t => ({
    name: t.name,
    status: t.status
  })));
  console.log('=== END APPLYING FILTER ===');
}

  canCancel(tour: Tour): boolean {
    try {
      if (!tour || !tour.status || !tour.scheduledDate) {
        return false;
      }
      
      const now = new Date();
      const tourDate = new Date(tour.scheduledDate);
      const hoursDiff = (tourDate.getTime() - now.getTime()) / (1000 * 60 * 60);
      
      const status = this.safeToLowerCase(tour.status);
      return status === 'published' && hoursDiff > 24;
    } catch (error) {
      console.error('Error in canCancel:', error);
      return false;
    }
  }

  cancelTour(tour: Tour): void {
    try {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        data: {
          title: 'Cancel Tour',
          message: `Are you sure you want to cancel "${tour.name}"? All tourists will receive bonus points.`
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.tourService.cancelTour(tour.id).subscribe({
            next: () => {
              this.snackBar.open('Tour cancelled successfully', 'Close', { duration: 3000 });
              this.loadMyTours();
            },
            error: (error) => {
              this.snackBar.open('Failed to cancel tour', 'Close', { duration: 3000 });
            }
          });
        }
      });
    } catch (error) {
      console.error('Error in cancelTour:', error);
      this.snackBar.open('Error cancelling tour', 'Close', { duration: 3000 });
    }
  }

  publishTour(tour: Tour): void {
    try {
      this.tourService.publishTour(tour.id).subscribe({
        next: () => {
          this.snackBar.open('Tour published successfully', 'Close', { duration: 3000 });
          this.loadMyTours();
        },
        error: (error) => {
          this.snackBar.open(error.error?.errors?.[0] || 'Failed to publish tour', 'Close', { duration: 3000 });
        }
      });
    } catch (error) {
      console.error('Error in publishTour:', error);
      this.snackBar.open('Error publishing tour', 'Close', { duration: 3000 });
    }
  }
}