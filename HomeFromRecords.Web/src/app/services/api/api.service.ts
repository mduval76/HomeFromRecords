import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();

    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  public getData(url: string): Observable<any> {
    return this.http.get(url, { headers: this.getHeaders() });
  }

  public postData(url: string, data: any): Observable<any> {
    return this.http.post(url, data, { headers: this.getHeaders() });
  }

  public putData(url: string, data: any): Observable<any> {
    return this.http.put(url, data, { headers: this.getHeaders() });
  }

  public deleteData(url: string): Observable<any> {
    return this.http.delete(url, { headers: this.getHeaders() });
  }
}
