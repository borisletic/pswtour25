import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  getMaliciousUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/users/malicious`);
  }

  blockUser(userId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/block`, {});
  }

  unblockUser(userId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/unblock`, {});
  }
}