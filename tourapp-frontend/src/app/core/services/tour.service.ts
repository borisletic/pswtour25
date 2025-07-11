import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Tour, TourFilter, CreateTourRequest, KeyPoint } from '../../models/tour.model';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private apiUrl = `${environment.apiUrl}/tour`;

  constructor(private http: HttpClient) {}

  getTours(filter?: TourFilter): Observable<Tour[]> {
    let params = new HttpParams();
    
    if (filter) {
      // Send numeric enum values instead of string representations
      if (filter.category !== undefined && filter.category !== null) {
        params = params.set('category', filter.category.toString());
      }
      if (filter.difficulty !== undefined && filter.difficulty !== null) {
        params = params.set('difficulty', filter.difficulty.toString());
      }
      if (filter.status) {
        params = params.set('status', filter.status);
      }
      if (filter.guideId) {
        params = params.set('guideId', filter.guideId);
      }
      if (filter.rewardedGuidesOnly === true) {
        params = params.set('rewardedGuidesOnly', 'true');
      }
    }

    return this.http.get<Tour[]>(this.apiUrl, { params });
  }

  getTourById(id: string): Observable<Tour> {
    return this.http.get<Tour>(`${this.apiUrl}/${id}`);
  }

  createTour(tour: CreateTourRequest): Observable<{ tourId: string }> {
    return this.http.post<{ tourId: string }>(this.apiUrl, tour);
  }

  addKeyPoint(tourId: string, keyPoint: Omit<KeyPoint, 'id'>): Observable<{ keyPointId: string }> {
    return this.http.post<{ keyPointId: string }>(`${this.apiUrl}/${tourId}/keypoints`, keyPoint);
  }

  publishTour(tourId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${tourId}/publish`, {});
  }

  cancelTour(tourId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${tourId}/cancel`);
  }

  rateTour(tourId: string, score: number, comment?: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${tourId}/rate`, { score, comment });
  }
}