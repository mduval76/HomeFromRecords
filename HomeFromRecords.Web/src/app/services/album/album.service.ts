import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api/api.service';
import { AlbumDto } from '../../models/album.model';

export interface PaginatedAlbums {
  items: AlbumDto[];   
  totalItems: number;
  currentPage: number;
  itemsPerPage: number;
}

@Injectable({
  providedIn: 'root'
})
export class AlbumService {
  private baseUrl = 'http://localhost:5138/album';

  constructor(private apiService: ApiService) { }

  public getAllAlbums(pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    return this.apiService.getData(`${this.baseUrl}/all?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getAlbumsByArtistId(artistId: string, albumFormat?: number, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    const formatParam = albumFormat !== undefined ? `&albumFormat=${albumFormat}` : '';
    return this.apiService.getData(`${this.baseUrl}/artistId?artistId=${artistId}${formatParam}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getAlbumsByCountry(country: string, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    return this.apiService.getData(`${this.baseUrl}/country?country=${country}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getAlbumsByRecordLabelId(labelId: string, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    return this.apiService.getData(`${this.baseUrl}/labelId?labelId=${labelId}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getAlbumsByMainFormat(format: number, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    return this.apiService.getData(`${this.baseUrl}/format?albumFormat=${format}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getSearchAlbums(query: string, albumFormat?: number, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    const formatParam = albumFormat !== undefined ? `&albumFormat=${albumFormat}` : '';
    return this.apiService.getData(`${this.baseUrl}/search?query=${query}${formatParam}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  public getRandomAlbums(count: number, pageNumber: number = 1, pageSize: number = 12): Observable<PaginatedAlbums> {
    return this.apiService.getData(`${this.baseUrl}/random?count=${count}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}