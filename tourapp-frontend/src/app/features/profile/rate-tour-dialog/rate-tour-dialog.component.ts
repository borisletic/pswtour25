import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-rate-tour-dialog',
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
    <h2 mat-dialog-title>Rate Tour: {{ data.tour.name }}</h2>
    <mat-dialog-content>
      <form [formGroup]="ratingForm">
        <div class="rating-stars">
          <mat-icon *ngFor="let star of stars; let i = index" 
                    [class.filled]="i < ratingForm.get('score')?.value"
                    (click)="setRating(i + 1)">
            {{ i < ratingForm.get('score')?.value ? 'star' : 'star_border' }}
          </mat-icon>
        </div>
        
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Comment {{ isCommentRequired() ? '(Required)' : '(Optional)' }}</mat-label>
          <textarea matInput formControlName="comment" rows="4" 
                    placeholder="Share your experience..."></textarea>
          <mat-error *ngIf="ratingForm.get('comment')?.hasError('required')">
            Comment is required for ratings 1 and 2
          </mat-error>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" 
              [disabled]="ratingForm.invalid"
              (click)="onSubmit()">Submit Rating</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .rating-stars {
      display: flex;
      justify-content: center;
      margin: 20px 0;
      font-size: 36px;
    }
    .rating-stars mat-icon {
      cursor: pointer;
      color: #ccc;
      transition: color 0.2s;
      margin: 0 4px;
    }
    .rating-stars mat-icon.filled {
      color: #ffd740;
    }
    .rating-stars mat-icon:hover {
      color: #ffd740;
    }
    .full-width {
      width: 100%;
      margin-top: 16px;
    }
    mat-dialog-content {
      min-width: 400px;
      padding: 24px !important;
    }
    mat-dialog-actions {
      padding: 16px 24px !important;
    }
  `]
})
export class RateTourDialogComponent {
  ratingForm: FormGroup;
  stars = [1, 2, 3, 4, 5];

  constructor(
    public dialogRef: MatDialogRef<RateTourDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder
  ) {
    this.ratingForm = this.fb.group({
      score: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['']
    });

    // Update comment validation when score changes
    this.ratingForm.get('score')?.valueChanges.subscribe(score => {
      this.updateCommentValidation(score);
    });
  }

  setRating(rating: number): void {
    this.ratingForm.patchValue({ score: rating });
  }

  isCommentRequired(): boolean {
    const score = this.ratingForm.get('score')?.value;
    return score === 1 || score === 2;
  }

  updateCommentValidation(score: number): void {
    const commentControl = this.ratingForm.get('comment');
    if (score === 1 || score === 2) {
      commentControl?.setValidators([Validators.required]);
    } else {
      commentControl?.clearValidators();
    }
    commentControl?.updateValueAndValidity();
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.ratingForm.valid) {
      this.dialogRef.close(this.ratingForm.value);
    }
  }
}