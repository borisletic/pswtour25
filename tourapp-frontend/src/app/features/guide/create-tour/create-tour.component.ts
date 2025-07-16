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
import { firstValueFrom } from 'rxjs';
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
  
  // Image handling properties
  imageStates: { [key: number]: { loading: boolean; error: boolean } } = {};
  
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
    
    // Remove image state for this key point
    delete this.imageStates[index];
    
    // Update indices for remaining image states
    const newImageStates: { [key: number]: { loading: boolean; error: boolean } } = {};
    Object.keys(this.imageStates).forEach(key => {
      const oldIndex = parseInt(key);
      if (oldIndex > index) {
        newImageStates[oldIndex - 1] = this.imageStates[oldIndex];
      } else if (oldIndex < index) {
        newImageStates[oldIndex] = this.imageStates[oldIndex];
      }
    });
    this.imageStates = newImageStates;
    
    // Update order for remaining key points
    this.keyPoints.controls.forEach((control, i) => {
      control.patchValue({ order: i + 1 });
    });
  }

  // Image handling methods
  validateImageUrl(index: number): void {
    const keyPoint = this.keyPoints.at(index);
    const imageUrl = keyPoint?.get('imageUrl')?.value;
    
    if (imageUrl && imageUrl.trim()) {
      this.imageStates[index] = { loading: true, error: false };
    } else {
      delete this.imageStates[index];
    }
  }

  onImageUrlChange(index: number): void {
    const keyPoint = this.keyPoints.at(index);
    const imageUrl = keyPoint?.get('imageUrl')?.value;
    
    if (imageUrl && imageUrl.trim()) {
      this.imageStates[index] = { loading: true, error: false };
    } else {
      delete this.imageStates[index];
    }
  }

  onImageLoad(index: number): void {
    if (this.imageStates[index]) {
      this.imageStates[index] = { loading: false, error: false };
    }
  }

  onImageError(index: number): void {
    if (this.imageStates[index]) {
      this.imageStates[index] = { loading: false, error: true };
    }
  }

  isValidImageUrl(url: string): boolean {
    try {
      new URL(url);
      return /\.(jpg|jpeg|png|gif|webp|svg)$/i.test(url);
    } catch {
      return false;
    }
  }

  private validateKeyPoints(keyPoints: any[]): boolean {
    console.log('Validating keyPoints:', keyPoints);
    
    for (let i = 0; i < keyPoints.length; i++) {
      const kp = keyPoints[i];
      console.log(`Validating keyPoint ${i + 1}:`, kp);
      
      if (!kp.name || kp.name.trim() === '') {
        console.error(`KeyPoint ${i + 1} missing name`);
        this.snackBar.open(`KeyPoint ${i + 1} must have a name`, 'Close', { duration: 3000 });
        return false;
      }
      
      if (!kp.description || kp.description.trim() === '') {
        console.error(`KeyPoint ${i + 1} missing description`);
        this.snackBar.open(`KeyPoint ${i + 1} must have a description`, 'Close', { duration: 3000 });
        return false;
      }
      
      if (typeof kp.latitude !== 'number' || isNaN(kp.latitude)) {
        console.error(`KeyPoint ${i + 1} invalid latitude:`, kp.latitude);
        this.snackBar.open(`KeyPoint ${i + 1} has invalid coordinates`, 'Close', { duration: 3000 });
        return false;
      }
      
      if (typeof kp.longitude !== 'number' || isNaN(kp.longitude)) {
        console.error(`KeyPoint ${i + 1} invalid longitude:`, kp.longitude);
        this.snackBar.open(`KeyPoint ${i + 1} has invalid coordinates`, 'Close', { duration: 3000 });
        return false;
      }
      
      if (!kp.order || kp.order <= 0) {
        console.error(`KeyPoint ${i + 1} invalid order:`, kp.order);
        this.snackBar.open(`KeyPoint ${i + 1} has invalid order`, 'Close', { duration: 3000 });
        return false;
      }
    }
    
    console.log('All keyPoints validated successfully');
    return true;
  }

  async onSubmit(): Promise<void> {
    if (this.tourForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.tourForm.value;

    const scheduledDate = new Date(formValue.scheduledDate);
    scheduledDate.setHours(12, 0, 0, 0); // Set to noon local time
    
    // Validate keypoints before proceeding
    if (formValue.keyPoints.length > 0 && !this.validateKeyPoints(formValue.keyPoints)) {
      this.loading = false;
      return;
    }
    
    try {
      // Create tour first
      const response = await firstValueFrom(this.tourService.createTour({
        name: formValue.name,
        description: formValue.description,
        difficulty: formValue.difficulty,
        category: formValue.category,
        price: formValue.price,
        scheduledDate: scheduledDate
      }));
      
      this.currentTourId = response.tourId;
      console.log('Tour created with ID:', this.currentTourId);
      
      // Add key points sequentially
      if (formValue.keyPoints.length > 0) {
        await this.addKeyPointsToTour(formValue.keyPoints);
      }
      
      this.loading = false;
      this.snackBar.open('Tour created successfully!', 'Close', { duration: 3000 });
      this.router.navigate(['/guide/tours']);
      
    } catch (error) {
      this.loading = false;
      console.error('Error creating tour:', error);
      this.snackBar.open('Failed to create tour', 'Close', { duration: 3000 });
    }
  }

  private async addKeyPointsToTour(keyPoints: any[]): Promise<void> {
    console.log('addKeyPointsToTour called with:', keyPoints);
    console.log('currentTourId:', this.currentTourId);
    
    try {
      // Add key points sequentially instead of in parallel
      for (let i = 0; i < keyPoints.length; i++) {
        const keyPoint = keyPoints[i];
        
        const keyPointData = {
          name: keyPoint.name,
          description: keyPoint.description,
          latitude: keyPoint.latitude,
          longitude: keyPoint.longitude,
          imageUrl: keyPoint.imageUrl,
          order: keyPoint.order
        };
        
        console.log(`Adding key point ${i + 1}/${keyPoints.length}:`, keyPointData);
        
        const response = await firstValueFrom(
          this.tourService.addKeyPoint(this.currentTourId!, keyPointData)
        );
        
        console.log(`KeyPoint ${i + 1} created successfully:`, response);
      }
      
      console.log('All key points added successfully');
      
    } catch (error) {
      console.error('Error adding key points:', error);
      throw error; // Re-throw to be caught by onSubmit
    }
  }
}