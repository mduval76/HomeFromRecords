import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageService } from '../../services/page/page.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent implements OnInit, OnDestroy {
  private breakpointSubscription!: Subscription;

  totalPages: number = 0;
  currentPage: number = 1;
  isLoading: boolean = false;
  surroundingPages: number[] = [];

  constructor(
    private pageService: PageService,
    private breakpointObserver: BreakpointObserver
  ) {}

  ngOnInit() {
   // this.isLoading = true;

    this.pageService.totalItems$.subscribe(totalItems => {
      this.pageService.itemsPerPage$.subscribe(itemsPerPage => {
        this.totalPages = Math.ceil(totalItems / itemsPerPage);
        this.pageService.setSurroundingPages();
      });
    });

    this.pageService.currentPage$.subscribe(currentPage => {
      this.currentPage = currentPage;
      this.pageService.setSurroundingPages();
    });

    this.pageService.surroundingPages$.subscribe(surroundingPages => {
      this.surroundingPages = surroundingPages;
    });

    this.breakpointSubscription = this.breakpointObserver
      .observe([Breakpoints.XSmall])
      .subscribe(result => {
        if (result.matches) {
          this.pageService.setSurroundingPagesCount(2);
        } else {
          this.pageService.setSurroundingPagesCount(3);
        }
      });

   // this.isLoading = false;
  }

  ngOnDestroy() {
    if (this.breakpointSubscription) {
      this.breakpointSubscription.unsubscribe();
    }
  }

  onPageChange(page: number): void {
    this.pageService.changePage(page);
  }
}
