import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { SharedService } from '../../services/shared/shared.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatMenuModule,
    MatSidenavModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  @ViewChild('drawer') drawer!: MatSidenav;

  isOpened = false;
  displayComponentActive: boolean = false;
  mainCriteria: string = 'Artist';
  currentSearchQuery: string = '';

  private subscriptions: Subscription = new Subscription();
  
  constructor(private sharedService: SharedService, private router: Router) {}

  ngOnInit(): void {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.displayComponentActive = event.url.includes('/catalog');
        if (!this.displayComponentActive && this.drawer.opened) {
          this.drawer.close();
          this.isOpened = false;
        }
      }
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  setMainSortCriteria(mainCriteria: string) {
    if (mainCriteria === 'Reset') {
      this.sharedService.resetSearchQuery();
    }
    else {
      this.sharedService.setMainSortCriteria(mainCriteria);
      this.mainCriteria = mainCriteria;
    }

  }

  setAlphaSortCriteria(alphaCriteria: string) {
    this.sharedService.setAlphaSortCriteria(alphaCriteria);
  }

  setPriceSortCriteria(priceCriteria: string) {
    this.sharedService.setPriceSortCriteria(priceCriteria);
  }

  setFormatSortCriteria(format: number) {
    this.sharedService.setFormatSortCriteria(format);
    this.resetPriceSortCriteria();
  }

  resetPriceSortCriteria() {
    this.sharedService.setPriceSortCriteria('none');
  }

  
}
