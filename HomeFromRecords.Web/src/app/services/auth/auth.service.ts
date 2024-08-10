import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { ApiService } from '../api/api.service';
import { environment } from '../../../environments/environment.development';
import { catchError, map } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  exp: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userRoleSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private apiService: ApiService) { }

  public get userRoleObservable() {
    return this.userRoleSubject.asObservable();
  }

  public get isAdmin$(): Observable<boolean> {
    this.setUserRoleFromToken();
    return this.userRoleSubject.asObservable().pipe(
      map(role => role === 'Admin')
    );
  }

  setUserRoleFromToken(): void {
    const token = localStorage.getItem('token');

    if (token && !this.isTokenExpired()) {
      try {
        const decodedToken = jwtDecode<DecodedToken>(token);
        const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        this.userRoleSubject.next(role);
      } 
      catch (error) {
        console.error('Error decoding token', error);
        this.userRoleSubject.next(null);
      }
    } 
    else {
      this.userRoleSubject.next(null);
    }
  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('token');

    if (!token)
      return true;
  
    const decodedToken = jwtDecode<DecodedToken>(token);
    const currentTime = Math.floor(Date.now() / 1000);
    return decodedToken.exp < currentTime;
  }

  updateUserRole(token: string | null): void {
    if (token) {
      localStorage.setItem('token', token);
    } 
    else {
      localStorage.removeItem('token');
    }
  }

  clearUserRole(): void {
    this.updateUserRole(null);
  }

  logoutUser(): Observable<any> {
    const url = `${environment.apiUrl}User/logout`;
    const token = localStorage.getItem('token');
    
    if (token) {
      return this.apiService.postData(url, {}).pipe(
        map(response => {
          console.log('Logout response:', response);
          this.clearUserRole();
          return { success: true, message: 'Logout successful' };
        }),
        catchError(error => {
          this.clearUserRole();
          return throwError(() => new Error('Logout failed: ' + (error.error?.message || error.message)));
        })
      );
    } 
    else {
      this.clearUserRole();
      return throwError(() => new Error('No user session to log out'));
    }
  }
}
