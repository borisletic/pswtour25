import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PurchaseService } from '../../../core/services/purchase.service';
import { TourService } from '../../../core/services/tour.service';
import { ProblemService } from '../../../core/services/problem.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RateTourDialogComponent } from '../rate-tour-dialog/rate-tour-dialog.component';
import { ReportProblemDialogComponent } from './report-problem-dialog.component';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-purchase-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './purchase-history.component.html',
  styleUrls: ['./purchase-history.component.scss']
})
export class PurchaseHistoryComponent implements OnInit {
  purchases: any[] = [];
  loading = false;
  displayedColumns = ['purchaseDate', 'tours', 'totalPrice', 'bonusUsed', 'actions'];

  constructor(
    private purchaseService: PurchaseService,
    private tourService: TourService,
    private problemService: ProblemService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadPurchaseHistory();
  }

  loadPurchaseHistory(): void {
    this.loading = true;
    this.purchaseService.getPurchaseHistory().subscribe({
      next: (purchases) => {
        this.purchases = purchases;
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load purchase history', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  canRate(tour: any): boolean {
    const tourDate = new Date(tour.scheduledDate);
    const now = new Date();
    const daysSinceTour = (now.getTime() - tourDate.getTime()) / (1000 * 60 * 60 * 24);
    
    return tourDate < now && daysSinceTour <= 30 && !tour.hasRated;
  }

  rateTour(tour: any): void {
    const dialogRef = this.dialog.open(RateTourDialogComponent, {
      data: { tour },
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.tourService.rateTour(tour.id, result.score, result.comment).subscribe({
          next: () => {
            this.snackBar.open('Tour rated successfully', 'Close', { duration: 3000 });
            this.loadPurchaseHistory();
          },
          error: (error) => {
            const message = error.error?.errors?.[0] || 'Failed to rate tour';
            this.snackBar.open(message, 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  reportProblem(tour: any): void {
    const dialogRef = this.dialog.open(ReportProblemDialogComponent, {
      data: { tour },
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.problemService.reportProblem(tour.id, result.title, result.description).subscribe({
          next: () => {
            this.snackBar.open('Problem reported successfully. The guide will be notified.', 'Close', { 
              duration: 4000 
            });
          },
          error: (error) => {
            const message = error.error?.errors?.[0] || 'Failed to report problem';
            this.snackBar.open(message, 'Close', { duration: 3000 });
          }
        });
      }
    });
  }
}