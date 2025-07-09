import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TourService } from '../../../core/services/tour.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Interest, TourDifficulty } from '../../../models/tour.model';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import * as L from 'leaflet';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './create-tour.component.html',
  styleUrl: './create-tour.component.scss'
})
export class CreateTourComponent implements OnInit, AfterViewInit {
  tourForm!: FormGroup;
  loading = false;
  map: L.Map | null = null;
  markers: L.Marker[] = [];
  currentTourId: string | null = null;
  
  interests = Object.keys(Interest)
    .filter(k => isNaN(Number(k)))
    .map(key => ({ value: Interest[key as keyof typeof Interest], label: key }));
  
  difficulties = Object.keys(TourDifficulty)
    .filter(k => isNaN(Number(k)))
    .map(key => ({ value: TourDifficulty[key as keyof typeof TourDifficulty], label: key }));

  constructor(
    private fb: FormBuilder,
    private tourService: TourService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.initializeMap(), 100);
  }

  get keyPoints(): FormArray<FormGroup> {
    return this.tourForm.get('keyPoints') as FormArray<FormGroup>;
  }

  initializeForm(): void {
    this.tourForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      difficulty: ['', Validators.required],
      category: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      scheduledDate: ['', Validators.required],
      keyPoints: this.fb.array([])
    });
  }

  initializeMap(): void {
    this.map = L.map('create-tour-map').setView([45.2671, 19.8335], 13); // Novi Sad coordinates
    
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);
    
    this.map.on('click', (e: L.LeafletMouseEvent) => {
      this.addKeyPoint(e.latlng.lat, e.latlng.lng);
    });
  }

  

  addKeyPoint(lat: number, lng: number): void {
    const keyPointForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      latitude: [lat, Validators.required],
      longitude: [lng, Validators.required],
      imageUrl: [''],
      order: [this.keyPoints.length + 1]
    });
    
    this.keyPoints.push(keyPointForm);
    
    // Add marker to map
    const marker = L.marker([lat, lng], { draggable: true })
      .addTo(this.map!)
      .bindPopup(`Key Point ${this.keyPoints.length}`);
    
    marker.on('dragend', (e) => {
      const newLatLng = e.target.getLatLng();
      const index = this.markers.indexOf(marker);
      this.keyPoints.at(index).patchValue({
        latitude: newLatLng.lat,
        longitude: newLatLng.lng
      });
    });
    
    this.markers.push(marker);
    this.updatePolyline();
  }

  removeKeyPoint(index: number): void {
    this.keyPoints.removeAt(index);
    this.map!.removeLayer(this.markers[index]);
    this.markers.splice(index, 1);
    
    // Update order numbers
    this.keyPoints.controls.forEach((control, i) => {
      control.patchValue({ order: i + 1 });
    });
    
    this.updatePolyline();
  }

  private polyline: L.Polyline | null = null;
  
  updatePolyline(): void {
    if (this.polyline) {
      this.map!.removeLayer(this.polyline);
    }
    
    if (this.markers.length > 1) {
      const latLngs = this.markers.map(m => m.getLatLng());
      this.polyline = L.polyline(latLngs, { color: 'blue' }).addTo(this.map!);
    }
  }

  async createTour(): Promise<void> {
    if (this.tourForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.tourForm.value;
    const tourData = {
      name: formValue.name,
      description: formValue.description,
      difficulty: formValue.difficulty,
      category: formValue.category,
      price: formValue.price,
      scheduledDate: new Date(formValue.scheduledDate)
    };

    try {
      const response = await this.tourService.createTour(tourData).toPromise();
      this.currentTourId = response!.tourId;
      
      // Add key points
      for (const keyPoint of formValue.keyPoints) {
        await this.tourService.addKeyPoint(this.currentTourId, keyPoint).toPromise();
      }
      
      this.snackBar.open('Tour created successfully!', 'Close', { duration: 3000 });
      
      // Ask if user wants to publish
      const snackBarRef = this.snackBar.open(
        'Tour created! Do you want to publish it now?', 
        'Publish', 
        { duration: 5000 }
      );
      
      snackBarRef.onAction().subscribe(() => {
        this.publishTour();
      });
      
    } catch (error) {
      this.snackBar.open('Failed to create tour', 'Close', { duration: 3000 });
    } finally {
      this.loading = false;
    }
  }

  async publishTour(): Promise<void> {
    if (!this.currentTourId) return;
    
    try {
      await this.tourService.publishTour(this.currentTourId).toPromise();
      this.snackBar.open('Tour published successfully!', 'Close', { duration: 3000 });
      this.router.navigate(['/guide/tours']);
    } catch (error) {
      this.snackBar.open('Failed to publish tour', 'Close', { duration: 3000 });
    }
  }

  onSubmit(): void {
    if (this.keyPoints.length < 2) {
      this.snackBar.open('Please add at least 2 key points', 'Close', { duration: 3000 });
      return;
    }
    
    this.createTour();
  }
}