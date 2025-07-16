import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Interest } from '../../models/tour.model';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

export interface TouristProfile {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  interests: Interest[];
  bonusPoints: number;
}

@Injectable({
  providedIn: 'root'
})
export class TouristService {
  private apiUrl = `${environment.apiUrl}/tourist`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getAuthHeaders() {
    const token = this.authService.getToken();
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }

  getProfile(): Observable<TouristProfile> {
    return this.http.get<TouristProfile>(`${this.apiUrl}/profile`, {
      headers: this.getAuthHeaders()
    });
  }

  updateInterests(interests: Interest[]): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/interests`, 
      { interests }, 
      { headers: this.getAuthHeaders() }
    );
  }
}