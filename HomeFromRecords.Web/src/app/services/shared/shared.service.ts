import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private alphaSortCriteria = new BehaviorSubject<string>('ascending');
  private formatSortCriteria = new BehaviorSubject<number>(666);
  private mainSortCriteria = new BehaviorSubject<string>('Artist');
  private priceSortCriteria = new BehaviorSubject<string>('none');
  private searchQuery = new BehaviorSubject<string>('');
  private refreshNeeded = new BehaviorSubject<boolean>(false);

  constructor() { }

  // Getters --------------------------------------------------------------
  getAlphaSortCriteria() {
    return this.alphaSortCriteria.asObservable();
  }

  getFormatSortCriteria() {
    return this.formatSortCriteria.asObservable();
  }

  getMainSortCriteria() {
    return this.mainSortCriteria.asObservable();
  }

  getPriceSortCriteria() {
    return this.priceSortCriteria.asObservable();
  }

  getRefreshNeeded() {
    return this.refreshNeeded.asObservable();
  }

  getSearchQuery() {
    return this.searchQuery.asObservable();
  }

  // Setters --------------------------------------------------------------
  setAlphaSortCriteria(criteria: string) {
    this.alphaSortCriteria.next(criteria);
  }

  setFormatSortCriteria(format: number) {
    this.formatSortCriteria.next(format);
  }

  setMainSortCriteria(criteria: string) {
    this.mainSortCriteria.next(criteria);
  }

  setPriceSortCriteria(criteria: string) {
    this.priceSortCriteria.next(criteria);
  }

  setRefreshNeeded() {
    this.refreshNeeded.next(true);
  }

  setSearchQuery(query: string) {
    this.searchQuery.next(query);
  }

  // Methods --------------------------------------------------------------
  resetSearchQuery() {
    this.searchQuery.next('');
    this.notifyDataRefreshNeeded();
  }

  notifyDataRefreshNeeded() {
    this.refreshNeeded.next(true);
  }
}
