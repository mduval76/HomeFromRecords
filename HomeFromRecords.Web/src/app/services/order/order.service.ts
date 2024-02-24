import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface CartItem {
  id: string;
  artistName: string;
  albumTitle: string;
  price: number;
  imgFileExt: string;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private itemsInCartSubject: BehaviorSubject<CartItem[]> = new BehaviorSubject<CartItem[]>([]);

  constructor() {}

   public addItemToCart(item: CartItem): void {
    const itemFound = this.itemsInCartSubject.value.find(cartItem => cartItem.id === item.id);
  
    if (!itemFound) {
      const currentItems = [...this.itemsInCartSubject.value, item];
      console.log('Adding item to cart:', item);
      console.log('Current items in cart:', currentItems);
      this.itemsInCartSubject.next(currentItems);
    } else {
      console.log("This item is no longer available.");
    }
  }

  public getItems(): Observable<CartItem[]> {
    return this.itemsInCartSubject.asObservable();
  }

  public removeItemFromCart(title: string): void {
    console.log('Removing item in SERVICE:', title);
    const currentItems = this.itemsInCartSubject.value.filter(cartItem => cartItem.albumTitle !== title);
    this.itemsInCartSubject.next(currentItems);
  }
}
