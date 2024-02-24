import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class EnumService {
  enums: any = {};

  constructor(private apiService: ApiService) { }

  fetchEnums(): Observable<any> {
    const enumsUrl = `${environment.apiUrl}Constants/enums`;
    return this.apiService.getData(enumsUrl);
  }
}
