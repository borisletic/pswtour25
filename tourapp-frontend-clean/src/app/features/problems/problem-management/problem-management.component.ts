import { Component, OnInit } from '@angular/core';
import { ProblemService } from '../../../core/services/problem.service';
import { AuthService } from '../../../core/services/auth.service';
import { Problem, ProblemStatus } from '../../../models/problem.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { ProblemDetailsDialogComponent } from '../problem-details-dialog/problem-details-dialog.component';
import { MatCard } from "@angular/material/card";
import { SharedModule } from "../../../shared/shared.module";

@Component({
  selector: 'app-problem-management',
  templateUrl: './problem-management.component.html',
  styleUrls: ['./problem-management.component.scss'],
  imports: [MatCard, SharedModule]
})
export class ProblemManagementComponent implements OnInit {
  problems: Problem[] = [];
  loading = false;
  userRole: string;
  
  displayedColumns = ['tour', 'tourist', 'title', 'status', 'createdAt', 'actions'];
  
  statusColors: { [key in ProblemStatus]: string } = {
    [ProblemStatus.Pending]: 'warn',
    [ProblemStatus.Resolved]: 'primary',
    [ProblemStatus.UnderReview]: 'accent',
    [ProblemStatus.Rejected]: 'warn'
  };

  constructor(
    private problemService: ProblemService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.userRole = this.authService.getCurrentUser().type;
  }

  ngOnInit(): void {
    this.loadProblems();
  }

  getStatusColor(status: ProblemStatus): string {
  return this.statusColors[status];
  }


  loadProblems(): void {
    this.loading = true;
    
    if (this.userRole === 'Guide') {
      this.problemService.getGuideProblems().subscribe({
        next: (problems) => {
          this.problems = problems;
          this.loading = false;
        },
        error: () => {
          this.snackBar.open('Failed to load problems', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
    } else if (this.userRole === 'Administrator') {
      this.problemService.getProblemsUnderReview().subscribe({
        next: (problems) => {
          this.problems = problems;
          this.loading = false;
        },
        error: () => {
          this.snackBar.open('Failed to load problems', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
    }
  }

  resolveProblem(problem: Problem): void {
    this.problemService.resolveProblem(problem.id).subscribe({
      next: () => {
        this.snackBar.open('Problem marked as resolved', 'Close', { duration: 3000 });
        this.loadProblems();
      },
      error: () => {
        this.snackBar.open('Failed to resolve problem', 'Close', { duration: 3000 });
      }
    });
  }

  sendForReview(problem: Problem): void {
    this.problemService.sendForReview(problem.id).subscribe({
      next: () => {
        this.snackBar.open('Problem sent for review', 'Close', { duration: 3000 });
        this.loadProblems();
      },
      error: () => {
        this.snackBar.open('Failed to send problem for review', 'Close', { duration: 3000 });
      }
    });
  }

  returnToPending(problem: Problem): void {
    this.problemService.returnToPending(problem.id).subscribe({
      next: () => {
        this.snackBar.open('Problem returned to guide', 'Close', { duration: 3000 });
        this.loadProblems();
      },
      error: () => {
        this.snackBar.open('Failed to return problem', 'Close', { duration: 3000 });
      }
    });
  }

  rejectProblem(problem: Problem): void {
    this.problemService.rejectProblem(problem.id).subscribe({
      next: () => {
        this.snackBar.open('Problem rejected', 'Close', { duration: 3000 });
        this.loadProblems();
      },
      error: () => {
        this.snackBar.open('Failed to reject problem', 'Close', { duration: 3000 });
      }
    });
  }

  viewDetails(problem: Problem): void {
    this.dialog.open(ProblemDetailsDialogComponent, {
      data: problem,
      width: '600px'
    });
  }
}