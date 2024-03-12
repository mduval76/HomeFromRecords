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

  calculateSurroundingPages(totalPages: number, currentPage: number): number[] {
    const surroundingPages = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) {
        surroundingPages.push(i);
      }
    } else {
      if (currentPage <= 4) {
        surroundingPages.push(1, 2, 3, 4, 5, 6, 7, 8, totalPages);
      } else if (currentPage >= totalPages - 3) {
        surroundingPages.push(1, totalPages - 7, totalPages - 6, totalPages - 5, totalPages - 4, totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
      } else {
        surroundingPages.push(1, currentPage - 3, currentPage - 2, currentPage - 1, currentPage, currentPage + 1, currentPage + 2, currentPage + 3, totalPages);
      }
    }
    return surroundingPages;
  }
}
