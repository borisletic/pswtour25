import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TouristService, TouristProfile } from '../../core/services/tourist.service';
import { Interest } from '../../models/tour.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="profile-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>My Profile</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div *ngIf="loading" class="loading-spinner">
            <mat-spinner></mat-spinner>
          </div>

          <form [formGroup]="profileForm" (ngSubmit)="onSubmit()" *ngIf="!loading">
            <div class="profile-info">
              <h3>Personal Information</h3>
              <p><strong>Name:</strong> {{profile?.firstName}} {{profile?.lastName}}</p>
              <p><strong>Email:</strong> {{profile?.email}}</p>
              <p><strong>Username:</strong> {{profile?.username}}</p>
              <p><strong>Bonus Points:</strong> {{profile?.bonusPoints}} EUR</p>
            </div>

            <div class="interests-section">
              <h3>My Interests</h3>
              <p>Select your interests to receive tour recommendations:</p>
              
              <div formArrayName="interests" class="interests-grid">
                <mat-checkbox 
                  *ngFor="let interest of availableInterests; let i = index"
                  (change)="onInterestChange($event, interest.value)"
                  [checked]="isInterestSelected(interest.value)">
                  {{interest.label}}
                </mat-checkbox>
              </div>
            </div>

            <div class="form-actions">
              <button mat-raised-button color="primary" type="submit" [disabled]="profileForm.invalid || updating">
                {{updating ? 'Updating...' : 'Update Interests'}}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .profile-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }

    .loading-spinner {
      display: flex;
      justify-content: center;
      padding: 40px;
    }

    .profile-info {
      margin-bottom: 30px;
      padding: 20px;
      background-color: #f5f5f5;
      border-radius: 8px;
    }

    .profile-info h3 {
      margin-top: 0;
      color: #333;
    }

    .profile-info p {
      margin: 8px 0;
    }

    .interests-section {
      margin-bottom: 30px;
    }

    .interests-section h3 {
      color: #333;
      margin-bottom: 10px;
    }

    .interests-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 15px;
      margin-top: 15px;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      padding-top: 20px;
      border-top: 1px solid #eee;
    }
  `]
})
export class ProfileComponent implements OnInit {
  profileForm: FormGroup;
  profile: TouristProfile | null = null;
  loading = false;
  updating = false;

  availableInterests = [
    { value: Interest.Nature, label: 'Nature' },
    { value: Interest.Art, label: 'Art' },
    { value: Interest.Sport, label: 'Sport' },
    { value: Interest.Shopping, label: 'Shopping' },
    { value: Interest.Food, label: 'Food' }
  ];

  constructor(
    private fb: FormBuilder,
    private touristService: TouristService,
    private snackBar: MatSnackBar
  ) {
    this.profileForm = this.fb.group({
      interests: this.fb.array([], [Validators.required])
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.touristService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.initializeInterestsForm();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading profile:', error);
        this.snackBar.open('Failed to load profile', 'Close', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  initializeInterestsForm(): void {
    if (!this.profile) return;

    const interestsArray = this.profileForm.get('interests') as FormArray;
    interestsArray.clear();

    // Initialize with selected interests
    this.profile.interests.forEach(() => {
      interestsArray.push(this.fb.control(true));
    });
  }

  isInterestSelected(interest: Interest): boolean {
    return this.profile?.interests.includes(interest) || false;
  }

  onInterestChange(event: any, interest: Interest): void {
    if (!this.profile) return;

    if (event.checked) {
      if (!this.profile.interests.includes(interest)) {
        this.profile.interests.push(interest);
      }
    } else {
      const index = this.profile.interests.indexOf(interest);
      if (index > -1) {
        this.profile.interests.splice(index, 1);
      }
    }
  }

  onSubmit(): void {
    if (this.profileForm.invalid || !this.profile) return;

    if (this.profile.interests.length === 0) {
      this.snackBar.open('Please select at least one interest', 'Close', { duration: 3000 });
      return;
    }

    this.updating = true;
    this.touristService.updateInterests(this.profile.interests).subscribe({
      next: () => {
        this.snackBar.open('Interests updated successfully!', 'Close', { duration: 3000 });
        this.updating = false;
      },
      error: (error) => {
        console.error('Error updating interests:', error);
        this.snackBar.open('Failed to update interests', 'Close', { duration: 3000 });
        this.updating = false;
      }
    });
  }
}