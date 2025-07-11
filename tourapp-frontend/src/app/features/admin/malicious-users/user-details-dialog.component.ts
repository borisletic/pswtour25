// user-details-dialog.component.ts
import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';

interface MaliciousUser {
  id: string;
  username: string;
  email: string;
  type: string;
  reason: string;
  count: number;
  isBlocked: boolean;
}

@Component({
  selector: 'app-user-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule
  ],
  template: `
    <div class="user-details-dialog">
      <h2 mat-dialog-title>
        <mat-icon>{{data.type === 'Tourist' ? 'person' : 'tour'}}</mat-icon>
        User Details
      </h2>
      
      <mat-dialog-content>
        <div class="user-info">
          <mat-card class="info-card">
            <mat-card-header>
              <mat-card-title>Basic Information</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="info-row">
                <strong>Username:</strong>
                <span>{{data.username}}</span>
              </div>
              <div class="info-row">
                <strong>Email:</strong>
                <span>{{data.email}}</span>
              </div>
              <div class="info-row">
                <strong>User Type:</strong>
                <mat-chip [color]="data.type === 'Tourist' ? 'primary' : 'accent'">
                  <mat-icon>{{data.type === 'Tourist' ? 'person' : 'tour'}}</mat-icon>
                  {{data.type}}
                </mat-chip>
              </div>
              <div class="info-row">
                <strong>Status:</strong>
                <mat-chip [color]="data.isBlocked ? 'warn' : 'accent'">
                  <mat-icon>{{data.isBlocked ? 'block' : 'check_circle'}}</mat-icon>
                  {{data.isBlocked ? 'Blocked' : 'Active'}}
                </mat-chip>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="violation-card">
            <mat-card-header>
              <mat-card-title>Violation Details</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="info-row">
                <strong>Reason for Being Malicious:</strong>
                <span>{{data.reason}}</span>
              </div>
              <div class="info-row">
                <strong>Violation Count:</strong>
                <mat-chip [color]="getCountColor()">
                  {{data.count}} {{data.type === 'Tourist' ? 'invalid problems' : 'cancelled tours'}}
                </mat-chip>
              </div>
              <div class="info-row">
                <strong>Severity Level:</strong>
                <mat-chip [color]="getSeverityColor()">
                  {{getSeverityLevel()}}
                </mat-chip>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="recommendations-card" *ngIf="!data.isBlocked">
            <mat-card-header>
              <mat-card-title>Recommendations</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="recommendation" *ngIf="data.count >= 15">
                <mat-icon color="warn">warning</mat-icon>
                <span>Consider blocking this user - high violation count</span>
              </div>
              <div class="recommendation" *ngIf="data.count >= 10 && data.count < 15">
                <mat-icon color="accent">info</mat-icon>
                <span>Monitor this user closely for further violations</span>
              </div>
              <div class="recommendation">
                <mat-icon color="primary">email</mat-icon>
                <span>Consider sending a warning email to the user</span>
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="blocked-info-card" *ngIf="data.isBlocked">
            <mat-card-header>
              <mat-card-title>
                <mat-icon color="warn">block</mat-icon>
                Blocked User Information
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <p>This user has been blocked from accessing the system.</p>
              <p>They have received an email notification about the block.</p>
              <p>You can unblock them using the unblock button in the main table.</p>
            </mat-card-content>
          </mat-card>
        </div>
      </mat-dialog-content>
      
      <mat-dialog-actions align="end">
        <button mat-button (click)="close()">Close</button>
        <button mat-raised-button color="primary" (click)="close()">OK</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .user-details-dialog {
      min-width: 500px;
      max-width: 600px;
    }

    .user-info {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin: 16px 0;
    }

    .info-card, .violation-card, .recommendations-card, .blocked-info-card {
      margin-bottom: 16px;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 8px 0;
      border-bottom: 1px solid #eee;
    }

    .info-row:last-child {
      border-bottom: none;
    }

    .info-row strong {
      color: #333;
      margin-right: 16px;
    }

    .recommendation {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 8px 0;
      padding: 8px;
      background-color: #f5f5f5;
      border-radius: 4px;
    }

    .blocked-info-card {
      background-color: #fff3e0;
      border-left: 4px solid #ff9800;
    }

    .blocked-info-card p {
      margin: 8px 0;
    }

    h2 {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    @media (max-width: 600px) {
      .user-details-dialog {
        min-width: 90vw;
      }
      
      .info-row {
        flex-direction: column;
        align-items: flex-start;
        gap: 4px;
      }
    }
  `]
})
export class UserDetailsDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<UserDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MaliciousUser
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  getCountColor(): string {
    if (this.data.count >= 20) return 'warn';
    if (this.data.count >= 15) return 'accent';
    return 'primary';
  }

  getSeverityColor(): string {
    if (this.data.count >= 20) return 'warn';
    if (this.data.count >= 15) return 'accent';
    return 'primary';
  }

  getSeverityLevel(): string {
    if (this.data.count >= 20) return 'Critical';
    if (this.data.count >= 15) return 'High';
    if (this.data.count >= 10) return 'Medium';
    return 'Low';
  }
}