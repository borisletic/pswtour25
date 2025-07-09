import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Tour } from '../../models/tour.model';

interface CartItem {
  tour: Tour;
  quantity: number;
}

interface PurchaseRequest {
  tourIds: string[];
  bonusPointsToUse: number;
}

interface PurchaseResponse {
  purchaseId: string;
  totalPaid: number;
  bonusPointsUsed: number;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private apiUrl = `${environment.apiUrl}/purchase`;
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCart();
  }

  private loadCart(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cartSubject.next(JSON.parse(savedCart));
    }
  }

  private saveCart(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartSubject.value));
  }

  addToCart(tour: Tour): void {
    const currentCart = this.cartSubject.value;
    const existingItem = currentCart.find(item => item.tour.id === tour.id);
    
    if (existingItem) {
      existingItem.quantity++;
    } else {
      currentCart.push({ tour, quantity: 1 });
    }
    
    this.cartSubject.next(currentCart);
    this.saveCart();
  }

  removeFromCart(tourId: string): void {
    const currentCart = this.cartSubject.value.filter(item => item.tour.id !== tourId);
    this.cartSubject.next(currentCart);
    this.saveCart();
  }

  clearCart(): void {
    this.cartSubject.next([]);
    localStorage.removeItem('cart');
  }

  getTotalPrice(): number {
    return this.cartSubject.value.reduce((total, item) => 
      total + (item.tour.price * item.quantity), 0
    );
  }

  purchase(bonusPointsToUse: number = 0): Observable<PurchaseResponse> {
    const tourIds = this.cartSubject.value.map(item => item.tour.id);
    const request: PurchaseRequest = { tourIds, bonusPointsToUse };
    
    return this.http.post<PurchaseResponse>(this.apiUrl, request);
  }

  getPurchaseHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history`);
  }
}