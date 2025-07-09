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

  loadMyTours(): void {
    this.loading = true;
    const guideId = this.authService.getCurrentUser().id;
    
    this.tourService.getTours({ guideId }).subscribe({
      next: (tours) => {
        this.tours = tours;
        this.applyFilter();
        this.loading = false;
      },
      error: (error) => {
        this.snackBar.open('Failed to load tours', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (this.filterStatus === 'all') {
      this.filteredTours = this.tours;
    } else {
      this.filteredTours = this.tours.filter(t => 
        t.status.toLowerCase() === this.filterStatus
      );
    }
  }

  canCancel(tour: Tour): boolean {
    const now = new Date();
    const tourDate = new Date(tour.scheduledDate);
    const hoursDiff = (tourDate.getTime() - now.getTime()) / (1000 * 60 * 60);
    return tour.status === 'Published' && hoursDiff > 24;
  }

  cancelTour(tour: Tour): void {
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
  }

  publishTour(tour: Tour): void {
    this.tourService.publishTour(tour.id).subscribe({
      next: () => {
        this.snackBar.open('Tour published successfully', 'Close', { duration: 3000 });
        this.loadMyTours();
      },
      error: (error) => {
        this.snackBar.open(error.error?.errors?.[0] || 'Failed to publish tour', 'Close', { duration: 3000 });
      }
    });
  }
}