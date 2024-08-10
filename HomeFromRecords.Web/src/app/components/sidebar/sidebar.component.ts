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
  activeFormat: number = 666;
  alphaAscendingActive: boolean = true;
  alphaDescendingActive: boolean = false;
  priceAscendingActive: boolean = false;
  priceDescendingActive: boolean = false;
  isOpened: boolean = false;
  displayComponentActive: boolean = false;
  mainCriteria: string = 'Artist';

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
      this.resetSortCriteria();
    }
    else {
      this.sharedService.setMainSortCriteria(mainCriteria);
      this.mainCriteria = mainCriteria;
    }
  }

  setAlphaSortCriteria(alphaCriteria: string) {
    if (alphaCriteria === 'ascending') {
      this.alphaAscendingActive = true;
      this.alphaDescendingActive = false;
    } 
    else if (alphaCriteria === 'descending') {
      this.alphaAscendingActive = false;
      this.alphaDescendingActive = true;
    }

    this.sharedService.setAlphaSortCriteria(alphaCriteria);
  }

  setPriceSortCriteria(priceCriteria: string) {
    if (priceCriteria === 'ascending') {
      this.priceAscendingActive = !this.priceAscendingActive;
      if (this.priceDescendingActive) {
        this.priceDescendingActive = false;
      }
    } 
    else if (priceCriteria === 'descending') {
      this.priceDescendingActive = !this.priceDescendingActive;
      if (this.priceAscendingActive) {
        this.priceAscendingActive = false;
      }
    }

    if (!this.priceAscendingActive && !this.priceDescendingActive) {
      this.sharedService.setPriceSortCriteria('none');
    } else {
      this.sharedService.setPriceSortCriteria(priceCriteria);
    }
  }

  setFormatSortCriteria(formatCriteria: number) {
    this.activeFormat = formatCriteria;
    this.sharedService.setFormatSortCriteria(this.activeFormat);
  }

  resetSortCriteria() {
    this.alphaAscendingActive = true;
    this.alphaDescendingActive = false;
    this.priceAscendingActive = false;
    this.priceDescendingActive = false;
    this.activeFormat = 666;
    this.sharedService.setAlphaSortCriteria('ascending');
    this.sharedService.setPriceSortCriteria('none');
  }

  
}
