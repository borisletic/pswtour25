import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Problem } from '../../models/problem.model';

@Injectable({
  providedIn: 'root'
})
export class ProblemService {
  private apiUrl = `${environment.apiUrl}/problem`;

  constructor(private http: HttpClient) {}

  reportProblem(tourId: string, title: string, description: string): Observable<{ problemId: string }> {
    return this.http.post<{ problemId: string }>(this.apiUrl, { tourId, title, description });
  }

  getTouristProblems(): Observable<Problem[]> {
    return this.http.get<Problem[]>(`${this.apiUrl}/tourist`);
  }

  getGuideProblems(): Observable<Problem[]> {
    return this.http.get<Problem[]>(`${this.apiUrl}/guide`);
  }

  getProblemsUnderReview(): Observable<Problem[]> {
    return this.http.get<Problem[]>(`${this.apiUrl}/admin/pending`);
  }

  resolveProblem(problemId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${problemId}/resolve`, {});
  }

  sendForReview(problemId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${problemId}/review`, {});
  }

  returnToPending(problemId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${problemId}/admin/return`, {});
  }

  rejectProblem(problemId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${problemId}/admin/reject`, {});
  }
}