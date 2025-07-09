import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
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
      error: () => {
        this.snackBar.open('Failed to load malicious users', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  blockUser(user: MaliciousUser): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Block User',
        message: `Are you sure you want to block ${user.username}?`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.adminService.blockUser(user.id).subscribe({
          next: () => {
            this.snackBar.open('User blocked successfully', 'Close', { duration: 3000 });
            this.loadMaliciousUsers();
          },
          error: () => {
            this.snackBar.open('Failed to block user', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  unblockUser(user: MaliciousUser): void {
    this.adminService.unblockUser(user.id).subscribe({
      next: () => {
        this.snackBar.open('User unblocked successfully', 'Close', { duration: 3000 });
        this.loadMaliciousUsers();
      },
      error: () => {
        this.snackBar.open('Failed to unblock user', 'Close', { duration: 3000 });
      }
    });
  }
}