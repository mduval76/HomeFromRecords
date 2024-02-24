import { Component, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { OrderService, CartItem } from '../../services/order/order.service';

export interface TableItem {
  position: number,
  image: any;
  artist: string
  album: string;
  price: number;
  cancel: any;
}

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    MatCardModule,
    MatTableModule,
    CurrencyPipe
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit{
  displayedColumns = ['cart-position', 'cart-image', 'cart-artist', 'cart-album', 'cart-price', 'cart-cancel'];
  dataSource: CartItem[] = [];
  items: any = {};

  constructor(private orderService: OrderService) {}

  ngOnInit() {
    this.orderService.getItems().subscribe(items => {
      console.log('OrderService items', items);
  
      this.items = items.map((dataSource, index) => ({
        position: index + 1,
        image: dataSource.imgFileExt,
        artist: dataSource.artistName,
        album: dataSource.albumTitle,
        price: dataSource.price,
        cancel: 'Remove'
      }));
  
      this.dataSource = items;
    });
  }

  getItems() {
  }

  getShipping() {
    return 25 * 1.66;
  }

  getTotalCost() {
    return this.dataSource.map(d => d.price).reduce((acc, value) => acc + value, 0);
  }

  getTotalWithShipping() {  
    return this.getTotalCost() + this.getShipping() + this.getTPS() + this.getTVQ();
  }

  getTPS() {
    return this.getTotalCost() * 0.05;
  }

  getTVQ() {
    return this.getTotalCost() * 0.09975;
  }

  removeItem(title: string) {
    console.log('Removing item in COMPONENT:', title);
    this.orderService.removeItemFromCart(title);
  }

  checkout() {}
}
