import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatGridListModule } from '@angular/material/grid-list';
import { ApiService } from '../../services/api/api.service';
import { FormatService } from '../../services/format/format.service';
import { OrderService, CartItem } from '../../services/order/order.service';
import { PageService } from '../../services/page/page.service';
import { SharedService } from '../../services/shared/shared.service';
import { DetailComponent } from '../detail/detail.component';
import { Subscription } from 'rxjs';
import { environment } from '../../../environments/environment.development';

type EnumDictionary = { [key: number]: string };

interface EnumItem {
  name: string;
  value: number;
}

@Component({
  selector: 'app-display',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatDialogModule,
    MatDividerModule,
    MatGridListModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './display.component.html',
  styleUrl: './display.component.scss'
})
export class DisplayComponent implements OnInit, OnDestroy {
  cols: number = 3;
  currentSearchQuery: string = '';
  data: any = [];
  defaultImg: string = '../../../assets/images/defaults/default.webp';
  enums: any = {};
  format: number = 666;
  formattedArtistName: string = '';
  globalIndex: number = 0;
  imageClasses: Record<string, string> = {};
  isLoading: boolean = false;
  surroundingPages: number[] = [];
  needsLeadingEllipse: boolean = false;
  needsTrailingEllipse: boolean = false;


  alphaSortCriteria: string = 'ascending';
  mainSortCriteria: string = 'Artist';
  priceSortCriteria: string = 'none';

  @Input() currentPage = 1;
  @Input() itemsPerPage = 12;
  @Input() totalItems!: number;
  @Output() pageChanged: EventEmitter<number> = new EventEmitter();

  private subscriptions: Subscription = new Subscription();

  constructor(
    private apiService: ApiService,
    private formatService: FormatService,
    private orderService: OrderService,
    private pageService: PageService,
    private sharedService: SharedService,
    private breakpointObserver: BreakpointObserver,
    public dialog: MatDialog
    ) {}

  ngOnInit() {
    this.setupSubscriptions();
    this.setupBreakpointObserver();
    this.fetchEnums();
    this.fetchData(this.format);
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageService.changePage(page);
    }
  }

  getIconUrl(format: string): string {
    const formatIcons: Record<string, string> = {
      'Vinyl': '../../../assets/images/icons/LP.png',
      'CD': '../../../assets/images/icons/CD.png',
      'Cassette': '../../../assets/images/icons/K7.png'
    };

    return `url('${formatIcons[format]}')`;
  }

  fetchData(format: number) {
    let baseUrl = `${environment.apiUrl}Album/`;
    let queryParams = `page=${this.currentPage}&itemsPerPage=${this.itemsPerPage}`;
    this.isLoading = true;

    if (this.currentSearchQuery) {
      baseUrl += 'search';
      queryParams += `&query=${encodeURIComponent(this.currentSearchQuery)}`;
      if (format !== 666) {
        queryParams += `&albumFormat=${format}`;
      }
    } 
    else {
      if (format === 666) {
        baseUrl += 'all';
      } 
      else {
        baseUrl += 'format';
        queryParams += `&albumFormat=${format}`;
      }
    }
  
    let url = `${baseUrl}?${queryParams}`;
    console.log('Fetching data from URL:', url);

    this.apiService.getData(url).subscribe({
      next: (response) => {
        console.log('Response:', response);
        this.totalItems = response.length;
        let sortedData = response.items;
        if (this.priceSortCriteria !== 'none') {
          sortedData = this.applyPriceSorting(response, this.priceSortCriteria);
        } 
        else {
          sortedData = this.sortData(response, this.mainSortCriteria, this.alphaSortCriteria === 'descending');
        }

        sortedData.forEach((album: any) => {
          this.updateImageClass(album.imgFileExt);
          album.format = this.enums.mainFormats[album.format];
          album.subFormat = this.enums.subFormats[album.subFormat];
          album.vinylSpeed = this.enums.vinylSpeeds[album.vinylSpeed];
          album.mediaGrade = this.enums.grades[album.mediaGrade];
          album.sleeveGrade = this.enums.grades[album.sleeveGrade];
          album.packageType = this.enums.packageTypes[album.packageType];
          album.artistGenre = this.enums.artistGenres[album.artistGenre];
          album.albumGenre = this.enums.albumGenres[album.albumGenre];
          album.albumLength = this.enums.albumLengths[album.albumLength];
          album.albumType = this.enums.albumTypes[album.albumType];
        });

        this.data = sortedData.slice((this.currentPage - 1) * this.itemsPerPage, this.currentPage * this.itemsPerPage);
        this.surroundingPages = this.pageService.calculateSurroundingPages(this.totalPages, this.currentPage);
        this.pageService.updateData(this.data);
        this.isLoading = false; 
      },
      error: (error) => {
        console.error(error);
        this.isLoading = false; 
      }
    });
  }

  fetchEnums() {
    const enumsUrl = `${environment.apiUrl}Constants/enums`;

    this.apiService.getData(enumsUrl).subscribe({
      next: (response) => {
        this.enums = {};

        for (const enumType in response) {
          if (response.hasOwnProperty(enumType)) {
            this.enums[enumType] = response[enumType].reduce((acc: EnumDictionary, enumItem: EnumItem) => {
              acc[enumItem.value] = enumItem.name;
              return acc;
            }, {});
          }
        }
      },
      error: (error) => {
        console.error('Error fetching enums:', error);
      }
    });
  }
  
  handleImageError(event: Event, defaultImageUrl: string) {
    const element = event.target as HTMLImageElement;
    element.src = defaultImageUrl;
  }

  sortData(data: any[], mainCriteria: string, reverseAlphabetical: boolean): any[] {
    let sortedData = [...data];
    sortedData.sort((a, b) => {
      if (mainCriteria === 'Artist') {
        return a.artistName.localeCompare(b.artistName);
      } else {
        return a.title.localeCompare(b.title);
      }
    });

    if (reverseAlphabetical) {
      sortedData.reverse();
    }

    return sortedData;
  }

  applyPriceSorting(data: any[], priceCriteria: string): any[] {
    data.sort((a, b) => {
      return priceCriteria === 'ascending' ? a.price - b.price : b.price - a.price;
    });

    let priceSortedData = [];
    let currentPrice = null;
    let currentPriceGroup = [];

    for (let item of data) {
      if (item.price !== currentPrice) {
        if (currentPriceGroup.length > 0) {
          priceSortedData.push(...this.sortData(currentPriceGroup, this.mainSortCriteria, this.alphaSortCriteria === 'descending'));
          currentPriceGroup = [];
        }
        currentPrice = item.price;
      }
      currentPriceGroup.push(item);
    }

    if (currentPriceGroup.length > 0) {
      priceSortedData.push(...this.sortData(currentPriceGroup, this.mainSortCriteria, this.alphaSortCriteria === 'descending'));
    }

    return priceSortedData;
  }

  getFormattedArtistName(str: string): string {
    this.formattedArtistName = this.formatService.formatArtistName(str);
    return this.formattedArtistName;
  }

  getImageClass(imageUrl: string): string {
    let imageClass = '';
    const img = new Image();
    img.onload = () => {
      const isSquare = img.width === 500 && img.height === 500;
      const isTall = img.height === 500 && img.width < 500;
  
      if (isSquare) {
        imageClass = 'square-format';
      } else if (isTall) {
        imageClass = 'tall-format';
      } else {
        imageClass = 'wide-format';
      }
    };
    img.src = imageUrl;
  
    return imageClass;
  }

  updateImageClass(imageUrl: string) {
    const img = new Image();
    img.onload = () => {
      if (img.width === 500 && img.height === 500) {
        this.imageClasses[imageUrl] = 'square-format';
      } else if (img.height === 500 && img.width < 500) {
        this.imageClasses[imageUrl] = 'tall-format';
      } else {
        this.imageClasses[imageUrl] = 'wide-format';
      }
    };
    
    img.onerror = () => {
      this.imageClasses[imageUrl] = 'default-format';
    };
    img.src = imageUrl;
  }

  addToCart(album: any): void {
    const cartItem: CartItem = {
      id: album.albumId,
      artistName: album.artistName,
      albumTitle: album.title,
      price: album.price,
      imgFileExt: album.imgFileExt,
      quantity: 1
    };
    
    this.orderService.addItemToCart(cartItem);
  }

  openDialog(clickedItem: any, itemIndex: number) {
    const globalIndex = (this.currentPage - 1) * this.itemsPerPage + itemIndex;
    const dialogRef = this.dialog.open(DetailComponent, {
      panelClass: 'border-dialog',
      data: { 
        selectedItem: clickedItem, 
        items: this.data, 
        currentIndex: itemIndex, 
        globalIndex: globalIndex,
        totalItems: this.totalItems,
        itemsPerPage: this.itemsPerPage,
        currentPage: this.currentPage
       }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }

  refreshDisplayData() {
    this.fetchData(this.format);
  };

  private setupSubscriptions() {
    const formatSubscription = this.sharedService.getFormatSortCriteria().subscribe(format => {
      this.format = format;
      this.currentPage = 1;
      this.fetchData(this.format);
    });

    const searchQuerySubscription = this.sharedService.getSearchQuery().subscribe(query => {
      this.currentSearchQuery = query;
      this.currentPage = 1;
      this.fetchData(this.format);
    });

    const mainSortSubscription = this.sharedService.getMainSortCriteria().subscribe(criteria => {
      this.mainSortCriteria = criteria;
      this.fetchData(this.format);
    });

    const alphaSortSubscription = this.sharedService.getAlphaSortCriteria().subscribe(criteria => {
      this.alphaSortCriteria = criteria;
      this.fetchData(this.format);
    });

    const priceSortSubscription = this.sharedService.getPriceSortCriteria().subscribe(criteria => {
      this.priceSortCriteria = criteria;
      this.fetchData(this.format);
    });

    const pageServiceSubscription = this.pageService.currentPage$.subscribe(page => {
      if (this.currentPage !== page) {
        this.currentPage = page;
        this.fetchData(this.format);
      }
    });

    const refreshNeededSubscription = this.sharedService.getRefreshNeeded().subscribe(needed => {
      if (needed) {
        this.refreshDisplayData();
      }
    });

    this.subscriptions.add(formatSubscription)
    this.subscriptions.add(searchQuerySubscription)
    this.subscriptions.add(mainSortSubscription)
    this.subscriptions.add(alphaSortSubscription)
    this.subscriptions.add(priceSortSubscription)
    this.subscriptions.add(pageServiceSubscription)
    this.subscriptions.add(refreshNeededSubscription);
  }

  searchForArtist(artistName: string): void {
    this.sharedService.setSearchQuery(artistName);
  }

  private setupBreakpointObserver() {
    const customBreakpoints: { [key: string]: number } = {
      '(min-width: 951px)': 3,
      '(min-width: 651px) and (max-width: 950px)': 2,
      '(max-width: 650px)': 1
    };

    this.breakpointObserver.observe(Object.keys(customBreakpoints)).subscribe(result => {
      for (const query of Object.keys(customBreakpoints)) {
        if (result.breakpoints[query]) {
          this.cols = customBreakpoints[query];
          break;
        }
      }
    });
  }
}
