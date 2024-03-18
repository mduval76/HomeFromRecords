import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PageService {
  private currentPage = new BehaviorSubject<number>(1);
  private itemsPerPage = new BehaviorSubject<number>(12);
  private currentData = new BehaviorSubject<any[]>([]);
  private leadingEllipse = new BehaviorSubject<boolean>(false);
  private trailingEllipse = new BehaviorSubject<boolean>(false);

  currentPage$ = this.currentPage.asObservable();
  itemsPerPage$ = this.itemsPerPage.asObservable();
  currentData$ = this.currentData.asObservable();
  leadingEllipse$ = this.leadingEllipse.asObservable();
  trailingEllipse$ = this.trailingEllipse.asObservable();

  constructor() { }

  changePage(page: number): void {
    this.currentPage.next(page);
  }

  updateData(newData: any[]): void {
    this.currentData.next(newData);
  }

  calculateSurroundingPages(totalPages: number, currentPage: number): number[] {
    const surroundingPages = [];
    if (totalPages <= 9) {
      for (let i = 1; i <= totalPages; i++) {
        surroundingPages.push(i);
      }
    } else {
      if (currentPage <= 4) {
        for (let i = 1; i <= 8; i++) {
          surroundingPages.push(i);
        }
        surroundingPages.push(totalPages);
        this.trailingEllipse.next(true);
      } else if (currentPage >= totalPages - 3) {
        surroundingPages.push(1);
        for (let i = totalPages - 7; i <= totalPages; i++) {
          surroundingPages.push(i);
        }
        this.leadingEllipse.next(true);
      } else {
        surroundingPages.push(1);
        for (let i = currentPage - 3; i <= currentPage + 3; i++) {
          surroundingPages.push(i);
        }
        surroundingPages.push(totalPages);
        this.leadingEllipse.next(true);
        this.trailingEllipse.next(true);
      }
    }
    return surroundingPages;
  }
}
