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

// Fix Leaflet icon issue
const iconRetinaUrl = 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png';
const iconUrl = 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png';
const shadowUrl = 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png';
const iconDefault = L.icon({
  iconRetinaUrl,
  iconUrl,
  shadowUrl,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  tooltipAnchor: [16, -28],
  shadowSize: [41, 41]
});
L.Marker.prototype.options.icon = iconDefault;

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
  styleUrls: ['./create-tour.component.scss']
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
    // Check if the map container exists
    const mapContainer = document.getElementById('create-tour-map');
    if (!mapContainer) {
      console.error('Map container not found');
      return;
    }

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
      .bindPopup(`Key Point ${this.keyPoints.length}`)
      .openPopup();
    
    // Update position when dragged
    marker.on('dragend', (e: any) => {
      const position = e.target.getLatLng();
      const index = this.markers.indexOf(marker);
      if (index !== -1) {
        this.keyPoints.at(index).patchValue({
          latitude: position.lat,
          longitude: position.lng
        });
      }
    });
    
    this.markers.push(marker);
  }

  removeKeyPoint(index: number): void {
    this.keyPoints.removeAt(index);
    
    // Remove marker from map
    if (this.markers[index]) {
      this.map!.removeLayer(this.markers[index]);
      this.markers.splice(index, 1);
    }
    
    // Update order for remaining key points
    this.keyPoints.controls.forEach((control, i) => {
      control.patchValue({ order: i + 1 });
    });
  }

  onSubmit(): void {
    if (this.tourForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.tourForm.value;
    
    // Create tour first
    this.tourService.createTour({
      name: formValue.name,
      description: formValue.description,
      difficulty: formValue.difficulty,
      category: formValue.category,
      price: formValue.price,
      scheduledDate: formValue.scheduledDate
    }).subscribe({
      next: (response) => {
        this.currentTourId = response.tourId;
        
        // Add key points
        if (formValue.keyPoints.length > 0) {
          this.addKeyPointsToTour(formValue.keyPoints);
        } else {
          this.loading = false;
          this.snackBar.open('Tour created successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/guide/tours']);
        }
      },
      error: (error) => {
        this.loading = false;
        this.snackBar.open('Failed to create tour', 'Close', { duration: 3000 });
      }
    });
  }

  private addKeyPointsToTour(keyPoints: any[]): void {
    let completed = 0;
    
    keyPoints.forEach((keyPoint) => {
      this.tourService.addKeyPoint(this.currentTourId!, {
        name: keyPoint.name,
        description: keyPoint.description,
        latitude: keyPoint.latitude,
        longitude: keyPoint.longitude,
        imageUrl: keyPoint.imageUrl,
        order: keyPoint.order
      }).subscribe({
        next: () => {
          completed++;
          if (completed === keyPoints.length) {
            this.loading = false;
            this.snackBar.open('Tour created successfully!', 'Close', { duration: 3000 });
            this.router.navigate(['/guide/tours']);
          }
        },
        error: (error) => {
          this.loading = false;
          this.snackBar.open('Failed to add key point', 'Close', { duration: 3000 });
        }
      });
    });
  }
}