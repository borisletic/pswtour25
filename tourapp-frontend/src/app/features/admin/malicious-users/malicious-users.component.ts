import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { AdminService } from '../../../core/services/admin.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

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
  selector: 'app-malicious-users',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './malicious-users.component.html',
  styleUrls: ['./malicious-users.component.scss']
})
export class MaliciousUsersComponent implements OnInit {
  maliciousUsers: MaliciousUser[] = [];
  loading = false;
  displayedColumns = ['username', 'email', 'type', 'reason', 'count', 'status', 'actions'];

  constructor(
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadMaliciousUsers();
  }

  loadMaliciousUsers(): void {
    this.loading = true;
    this.adminService.getMaliciousUsers().subscribe({
      next: (users) => {
        this.maliciousUsers = users;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading malicious users:', error);
        this.snackBar.open('Failed to load malicious users', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  blockUser(user: MaliciousUser): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Block User',
        message: `Are you sure you want to block ${user.username}? This will prevent them from logging into the system and they will receive an email notification.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.adminService.blockUser(user.id).subscribe({
          next: () => {
            this.snackBar.open(`User ${user.username} blocked successfully`, 'Close', { duration: 3000 });
            this.loadMaliciousUsers();
          },
          error: (error) => {
            console.error('Error blocking user:', error);
            this.snackBar.open('Failed to block user', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  unblockUser(user: MaliciousUser): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Unblock User',
        message: `Are you sure you want to unblock ${user.username}? They will be able to log in again.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.adminService.unblockUser(user.id).subscribe({
          next: () => {
            this.snackBar.open(`User ${user.username} unblocked successfully`, 'Close', { duration: 3000 });
            this.loadMaliciousUsers();
          },
          error: (error) => {
            console.error('Error unblocking user:', error);
            this.snackBar.open('Failed to unblock user', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  // Statistics methods for the summary cards
  getMaliciousTourists(): number {
    return this.maliciousUsers.filter(user => user.type === 'Tourist').length;
  }

  getMaliciousGuides(): number {
    return this.maliciousUsers.filter(user => user.type === 'Guide').length;
  }

  getBlockedUsers(): number {
    return this.maliciousUsers.filter(user => user.isBlocked).length;
  }
}