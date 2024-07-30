import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PageService {
  private currentData = new BehaviorSubject<any[]>([]);
  private currentPage = new BehaviorSubject<number>(1);
  private itemsPerPage = new BehaviorSubject<number>(12);
  private surroundingPages = new BehaviorSubject<number[]>([]);
  private surroundingPagesCount = new BehaviorSubject<number>(3); 
  private totalItems = new BehaviorSubject<number>(0);
  private totalPages = new BehaviorSubject<number>(0);

  currentData$ = this.currentData.asObservable();
  currentPage$ = this.currentPage.asObservable();
  itemsPerPage$ = this.itemsPerPage.asObservable();
  surroundingPages$ = this.surroundingPages.asObservable();
  totalItems$ = this.totalItems.asObservable();
  totalPages$ = this.totalPages.asObservable();
  surroundingPagesCount$ = this.surroundingPagesCount.asObservable();

  constructor() { }

  setTotalItems(total: number): void {
    this.totalItems.next(total);
  }

  setTotalPages(totalPages: number): void {
    this.totalPages.next(totalPages);
  }

  setItemsPerPage(count: number): void {
    this.itemsPerPage.next(count);
  }

  setCurrentData(data: any[]): void {
    this.currentData.next(data);
  }

  changePage(page: number): void {
    this.currentPage.next(page);
  }

  updateData(newData: any[]): void {
    this.currentData.next(newData);
  }

  setSurroundingPagesCount(count: number): void {
    this.surroundingPagesCount.next(count);
    this.setSurroundingPages();
  }

  setSurroundingPages() {
    const totalPages = Math.ceil(this.totalItems.value / this.itemsPerPage.value);
    const currentPage = this.currentPage.value;
    const surroundingPagesCount = this.surroundingPagesCount.value;
    const surroundingPages = this.calculateSurroundingPages(totalPages, currentPage, surroundingPagesCount);
    this.surroundingPages.next(surroundingPages);
  }

  calculateSurroundingPages(totalPages: number, currentPage: number, surroundingPagesCount: number): number[] {
    const surroundingPages = [];
    const pagesBefore = Math.max(1, currentPage - surroundingPagesCount);
    const pagesAfter = Math.min(totalPages, currentPage + surroundingPagesCount);

    if (totalPages <= 9) {
      for (let i = 1; i <= totalPages; i++) {
        surroundingPages.push(i);
      }
    } else {
      if (currentPage <= surroundingPagesCount + 1) {
        for (let i = 1; i <= surroundingPagesCount * 2 + 1; i++) {
          surroundingPages.push(i);
        }
        surroundingPages.push(totalPages);
      } else if (currentPage >= totalPages - surroundingPagesCount) {
        surroundingPages.push(1);
        for (let i = totalPages - surroundingPagesCount * 2; i <= totalPages; i++) {
          surroundingPages.push(i);
        }
      } else {
        surroundingPages.push(1);
        for (let i = pagesBefore; i <= pagesAfter; i++) {
          surroundingPages.push(i);
        }
        surroundingPages.push(totalPages);
      }
    }
    return surroundingPages;
  }
}
