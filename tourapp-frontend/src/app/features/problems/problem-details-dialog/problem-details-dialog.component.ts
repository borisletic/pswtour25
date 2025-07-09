import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent } from '@angular/material/dialog';
import { Problem } from '../../../models/problem.model';
import { MatChip } from "@angular/material/chips";
import { MatDivider } from "@angular/material/divider";
import { MatList } from "@angular/material/list";
import { SharedModule } from "../../../shared/shared.module";

@Component({
  selector: 'app-problem-details-dialog',
  template: `
    <h2 mat-dialog-title>Problem Details</h2>
    <mat-dialog-content>
      <div class="problem-details">
        <h3>{{ data.title }}</h3>
        <p><strong>Tour:</strong> {{ data.tourName }}</p>
        <p><strong>Reported by:</strong> {{ data.touristName }}</p>
        <p><strong>Status:</strong> 
          <mat-chip [color]="getStatusColor(data.status)">
            {{ data.status }}
          </mat-chip>
        </p>
        <p><strong>Reported on:</strong> {{ data.createdAt | date:'medium' }}</p>
        
        <mat-divider></mat-divider>
        
        <h4>Description</h4>
        <p>{{ data.description }}</p>
        
        <mat-divider></mat-divider>
        
        <h4>Status History</h4>
        <mat-list>
          <mat-list-item *ngFor="let event of data.events">
            <mat-icon matListItemIcon>history</mat-icon>
            <div matListItemTitle>
              {{ getEventDescription(event) }}
            </div>
            <div matListItemMeta>
              {{ event.occurredAt | date:'short' }}
            </div>
          </mat-list-item>
        </mat-list>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="close()">Close</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .problem-details {
      min-width: 400px;
      max-width: 600px;
    }
    mat-divider {
      margin: 16px 0;
    }
  `],
  imports: [MatChip, MatDialogContent, MatDivider, MatList, SharedModule]
})
export class ProblemDetailsDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ProblemDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Problem
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'Pending': 'warn',
      'Resolved': 'primary',
      'UnderReview': 'accent',
      'Rejected': 'warn'
    };
    return colors[status] || '';
  }

  getEventDescription(event: any): string {
    if (event.type === 'ProblemCreatedEvent') {
      return 'Problem created';
    } else if (event.type === 'ProblemStatusChangedEvent') {
      return `Status changed from ${event.oldStatus} to ${event.newStatus}`;
    }
    return 'Unknown event';
  }
}