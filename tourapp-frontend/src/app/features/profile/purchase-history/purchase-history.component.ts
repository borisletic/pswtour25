import { Component, OnInit } from '@angular/core';
import { PurchaseService } from '../../../core/services/purchase.service';
import { TourService } from '../../../core/services/tour.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { RateTourDialogComponent } from '../rate-tour-dialog/rate-tour-dialog.component';

@Component({
  selector: 'app-purchase-history',
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
    // Navigate to problem reporting page
    // Implementation depends on routing structure
  }
}