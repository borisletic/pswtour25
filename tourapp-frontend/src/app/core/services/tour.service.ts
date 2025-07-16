import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Tour, TourFilter, CreateTourRequest, KeyPoint } from '../../models/tour.model';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private apiUrl = `${environment.apiUrl}/tour`;

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  getTours(filter?: TourFilter): Observable<Tour[]> {
    let params = new HttpParams();
    
    console.log('getTours called with filter:', filter);
    
    // Only add parameters if they have values - don't send default status
    if (filter) {
      if (filter.category !== undefined && filter.category !== null) {
        params = params.set('category', filter.category.toString());
        console.log('Added category parameter:', filter.category.toString());
      }
      if (filter.difficulty !== undefined && filter.difficulty !== null) {
        params = params.set('difficulty', filter.difficulty.toString());
        console.log('Added difficulty parameter:', filter.difficulty.toString());
      }
      if (filter.guideId) {
        params = params.set('guideId', filter.guideId);
        console.log('Added guideId parameter:', filter.guideId);
      }
      if (filter.status) {
        params = params.set('status', filter.status);
        console.log('Added status parameter:', filter.status);
      }
      if (filter.rewardedGuidesOnly === true) {
        params = params.set('rewardedGuidesOnly', 'true');
        console.log('Added rewardedGuidesOnly parameter: true');
      }
    }

    console.log('Final params:', params.toString());
    console.log('Final URL:', `${this.apiUrl}?${params.toString()}`);

    return this.http.get<Tour[]>(this.apiUrl, { params });
  }

  getTourById(id: string): Observable<Tour> {
    return this.http.get<Tour>(`${this.apiUrl}/${id}`);
  }

  createTour(tour: CreateTourRequest): Observable<{ tourId: string }> {
    return this.http.post<{ tourId: string }>(
      this.apiUrl, 
      tour, 
      { headers: this.getAuthHeaders() }
    );
  }

  addKeyPoint(tourId: string, keyPoint: Omit<KeyPoint, 'id'>): Observable<{ keyPointId: string }> {
    return this.http.post<{ keyPointId: string }>(
      `${this.apiUrl}/${tourId}/keypoints`, 
      keyPoint, 
      { headers: this.getAuthHeaders() }
    );
  }

  publishTour(tourId: string): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${tourId}/publish`, 
      {}, 
      { headers: this.getAuthHeaders() }
    );
  }

  cancelTour(tourId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${tourId}/cancel`, 
      { headers: this.getAuthHeaders() }
    );
  }

  rateTour(tourId: string, score: number, comment: string): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.post(`${this.apiUrl}/${tourId}/rate`, { score, comment }, { headers });
  }

  getToursByRewardedGuides(): Observable<Tour[]> {
    return this.http.get<Tour[]>(`${this.apiUrl}/rewarded-guides`);
  }

  isRewardedGuide(guideId: string): Observable<{isRewarded: boolean}> {
    return this.http.get<{isRewarded: boolean}>(`${this.apiUrl}/guides/${guideId}/is-rewarded`);
  }

  getGuidesRewardStatus(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/guides/reward-status`);
  }

  /**
   * Test metoda za pokretanje mesečnog job-a ručno
   */
  testRewardJob(): Observable<{message: string}> {
    return this.http.post<{message: string}>(`${this.apiUrl}/test-reward-job`, {});
  }
}