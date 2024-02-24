import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PageService {
  private currentPage = new BehaviorSubject<number>(1);
  private itemsPerPage = new BehaviorSubject<number>(12);
  private currentData = new BehaviorSubject<any[]>([]);

  currentPage$ = this.currentPage.asObservable();
  itemsPerPage$ = this.itemsPerPage.asObservable();
  currentData$ = this.currentData.asObservable();

  constructor() { }

  changePage(page: number): void {
    this.currentPage.next(page);
  }

  updateData(newData: any[]): void {
    this.currentData.next(newData);
  }
}
