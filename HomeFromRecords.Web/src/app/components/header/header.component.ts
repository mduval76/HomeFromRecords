import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { FormatService } from '../../services/format/format.service';
import { OrderService } from '../../services/order/order.service';
import { SharedService } from '../../services/shared/shared.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatMenuModule,
    MatToolbarModule,
    RouterModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  searchQuery: string = '';
  isAdmin: boolean = false;
  
  @ViewChild(MatMenuTrigger) trigger!: MatMenuTrigger;
  
  constructor(
    public authService: AuthService,
    public formatService: FormatService,
    public orderService: OrderService,
    private sharedService: SharedService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.userRoleObservable.subscribe(role => {
      if (this.authService.isTokenExpired()) {
        this.isAdmin = false;
        this.authService.updateUserRole(role);
      }
      this.isAdmin = role === 'Admin';
    });

    this.sharedService.getSearchQuery().subscribe(query => {
      this.searchQuery = query;
    });
  }

  @HostListener('window:resize')
  onClear() {
    if (this.searchQuery.length === 0) {
      this.sharedService.setSearchQuery('');
    }
  }

  onResize() {
    if (this.trigger && this.trigger.menuOpen) {
      this.trigger.closeMenu();
    }
  }

  onSearch(query: string) {
    this.sharedService.setSearchQuery(query);
    this.router.navigate(['/catalog']);
  }
}