import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ReportProblemData {
  tour: any;
}

export interface ReportProblemResult {
  title: string;
  description: string;
}

@Component({
  selector: 'app-report-problem-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="dialog-container">
      <div class="dialog-header">
        <h2 mat-dialog-title>
          <mat-icon>report_problem</mat-icon>
          Report Problem
        </h2>
        <button mat-icon-button mat-dialog-close>
          <mat-icon>close</mat-icon>
        </button>
      </div>

      <mat-dialog-content class="dialog-content">
        <div class="tour-info">
          <h3>{{ data.tour.name }}</h3>
          <p class="tour-date">{{ data.tour.scheduledDate | date:'medium' }}</p>
        </div>

        <form [formGroup]="problemForm" class="problem-form">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Problem Title</mat-label>
            <input matInput formControlName="title" placeholder="Brief description of the issue">
            <mat-error *ngIf="problemForm.get('title')?.hasError('required')">
              Title is required
            </mat-error>
            <mat-error *ngIf="problemForm.get('title')?.hasError('minlength')">
              Title must be at least 5 characters
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Detailed Description</mat-label>
            <textarea matInput 
                      formControlName="description" 
                      rows="4"
                      placeholder="Please provide detailed information about the problem you experienced...">
            </textarea>
            <mat-error *ngIf="problemForm.get('description')?.hasError('required')">
              Description is required
            </mat-error>
            <mat-error *ngIf="problemForm.get('description')?.hasError('minlength')">
              Description must be at least 20 characters
            </mat-error>
          </mat-form-field>
        </form>
      </mat-dialog-content>

      <mat-dialog-actions class="dialog-actions">
        <button mat-button mat-dialog-close>Cancel</button>
        <button mat-raised-button 
                color="warn" 
                [disabled]="problemForm.invalid"
                (click)="onSubmit()">
          Submit Report
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .dialog-container {
      max-width: 500px;
    }

    .dialog-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;

      h2 {
        display: flex;
        align-items: center;
        gap: 8px;
        margin: 0;
        color: #d32f2f;

        mat-icon {
          font-size: 24px;
        }
      }
    }

    .dialog-content {
      padding: 0 24px 24px 24px;
    }

    .tour-info {
      background-color: #f5f5f5;
      padding: 16px;
      border-radius: 8px;
      margin-bottom: 24px;
      border-left: 4px solid #2196f3;

      h3 {
        margin: 0 0 8px 0;
        color: #333;
        font-size: 18px;
      }

      .tour-date {
        margin: 0;
        color: #666;
        font-size: 14px;
      }
    }

    .problem-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .full-width {
      width: 100%;
    }

    .dialog-actions {
      padding: 16px 24px;
      justify-content: flex-end;
      gap: 12px;
    }
  `]
})
export class ReportProblemDialogComponent {
  problemForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ReportProblemDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReportProblemData
  ) {
    this.problemForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(20)]]
    });
  }

  onSubmit(): void {
    if (this.problemForm.valid) {
      const result: ReportProblemResult = {
        title: this.problemForm.value.title,
        description: this.problemForm.value.description
      };
      this.dialogRef.close(result);
    }
  }
}