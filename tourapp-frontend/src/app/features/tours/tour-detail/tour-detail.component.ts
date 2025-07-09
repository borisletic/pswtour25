import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TourService } from '../../../core/services/tour.service';
import { PurchaseService } from '../../../core/services/purchase.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tour } from '../../../models/tour.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import * as L from 'leaflet';

@Component({
  selector: 'app-tour-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatSnackBarModule
  ],
  templateUrl: './tour-detail.component.html',
  styleUrls: ['./tour-detail.component.scss']
})
export class TourDetailComponent implements OnInit, OnDestroy, AfterViewInit {
  tour: Tour | null = null;
  loading = true;
  map: L.Map | null = null;
  markers: L.Marker[] = [];
  
  private destroy$ = new Subject<void>();
  
  constructor(
    private route: ActivatedRoute,
    private tourService: TourService,
    private purchaseService: PurchaseService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit(): void {
    const tourId = this.route.snapshot.paramMap.get('id');
    if (tourId) {
      this.loadTour(tourId);
    }
  }

  ngAfterViewInit(): void {
    // Map will be initialized after tour is loaded
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.map) {
      this.map.remove();
    }
  }
  
  loadTour(id: string): void {
    this.tourService.getTourById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (tour) => {
          this.tour = tour;
          this.loading = false;
          setTimeout(() => this.initializeMap(), 100);
        },
        error: (error) => {
          console.error('Error loading tour:', error);
          this.snackBar.open('Failed to load tour details', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
  }
  
  initializeMap(): void {
    if (!this.tour || !this.tour.keyPoints.length) return;
    
    // Initialize map centered on first key point
    const firstPoint = this.tour.keyPoints[0];
    this.map = L.map('tour-map').setView([firstPoint.latitude, firstPoint.longitude], 13);
    
    // Add tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);
    
    // Add markers for each key point
    const latLngs: L.LatLng[] = [];
    
    this.tour.keyPoints.forEach((keyPoint, index) => {
      const marker = L.marker([keyPoint.latitude, keyPoint.longitude])
        .addTo(this.map!)
        .bindPopup(`
          <div>
            <h4>${index + 1}. ${keyPoint.name}</h4>
            <p>${keyPoint.description}</p>
          </div>
        `);
      
      this.markers.push(marker);
      latLngs.push(L.latLng(keyPoint.latitude, keyPoint.longitude));
    });
    
    // Draw route between key points
    if (latLngs.length > 1) {
      L.polyline(latLngs, { color: 'blue', weight: 3 }).addTo(this.map);
    }
    
    // Fit map to show all markers
    const bounds = L.latLngBounds(latLngs);
    this.map.fitBounds(bounds, { padding: [50, 50] });
  }
  
  addToCart(): void {
    if (!this.tour) return;
    
    if (!this.authService.isAuthenticated()) {
      this.snackBar.open('Please login to add tours to cart', 'Close', { duration: 3000 });
      return;
    }
    
    if (!this.authService.hasRole('Tourist')) {
      this.snackBar.open('Only tourists can purchase tours', 'Close', { duration: 3000 });
      return;
    }
    
    this.purchaseService.addToCart(this.tour);
    this.snackBar.open('Tour added to cart', 'Close', { duration: 2000 });
  }
  
  isTourist(): boolean {
    return this.authService.hasRole('Tourist');
  }
}