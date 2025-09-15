import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '../../services/api/api.service';
import { FormatService } from '../../services/format/format.service';
import { OrderService, CartItem } from '../../services/order/order.service';
import { PageService } from '../../services/page/page.service';
import { AlbumService, PaginatedAlbums } from '../../services/album/album.service';
import { SharedService } from '../../services/shared/shared.service';
import { DetailComponent } from '../detail/detail.component';
import { PaginationComponent } from '../pagination/pagination.component';
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
    MatProgressSpinnerModule,
    NgOptimizedImage,
    PaginationComponent
  ],
  templateUrl: './display.component.html',
  styleUrl: './display.component.scss'
})
export class DisplayComponent implements OnInit, OnDestroy {
  cols = 3;
  currentSearchQuery = '';
  appliedSearchQuery = '';
  data: any[] = [];
  defaultImg = '../../../assets/images/defaults/default.webp';
  enums: Record<string, EnumDictionary> = {};
  format = 666;
  formattedArtistName = '';
  imageClasses: Record<string, string> = {};
  isLoading = false;
  currentPage = 1;
  itemsPerPage = 12;
  totalItems = 0;

  alphaSortCriteria = 'ascending';
  mainSortCriteria = 'Artist';
  priceSortCriteria = 'none';
  alphaSortCriteria = 'ascending';
  mainSortCriteria = 'Artist';
  priceSortCriteria = 'none';

  private subscriptions = new Subscription();

  constructor(
    private apiService: ApiService,
    private albumService: AlbumService,
    private formatService: FormatService,
    private orderService: OrderService,
    private pageService: PageService,
    private sharedService: SharedService,
    public dialog: MatDialog
    ) {}

  ngOnInit() {
    this.fetchData();
    this.fetchData();
    this.fetchEnums();
    this.setupSubscriptions();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  getIconUrl(format: string): string {
    const formatIcons: Record<string, string> = {
      'Vinyl': '../../../assets/images/icons/LP.png',
      'CD': '../../../assets/images/icons/CD.png',
      'Cassette': '../../../assets/images/icons/K7.png'
    };

    return `url('${formatIcons[format]}')`;
  }

  fetchData(format?: number, searchQuery?: string) {
  const queryToApply = searchQuery ?? this.appliedSearchQuery;
  const formatToApply = format ?? 666; // default format

  // Base URL
  let baseUrl = `${environment.apiUrl}Album/`;
  let queryParams = `page=${this.currentPage}&itemsPerPage=${this.itemsPerPage}`;

  if (queryToApply) {
    // Searching by query
    baseUrl += 'search';
    queryParams += `&query=${encodeURIComponent(queryToApply)}`;
  } else if (formatToApply !== 666) {
    // Filtering by format
    baseUrl += 'format';
    queryParams += `&albumFormat=${formatToApply}`;
  } else {
    // Default: get paged albums
    baseUrl += 'paged';
  }

  const url = `${baseUrl}?${queryParams}`;
  this.isLoading = true;

  this.apiService.getData(url).subscribe({
    next: (response: any) => {
      // Expecting a PagedResult structure
      if (!response || !Array.isArray(response.items)) {
        console.error('Invalid API response', response);
        this.isLoading = false;
        return;
      }

      // Assign items and total count
      this.data = response.items;
      this.totalItems = response.totalItems;

      // Map enums and update image classes
      this.data.forEach((album: any) => {
        this.updateImageClass(album.imgFileExt);
        album.format = this.enums['mainFormats']?.[album.format] ?? album.format;
        album.subFormat = this.enums['subFormats']?.[album.subFormat] ?? album.subFormat;
        album.vinylSpeed = this.enums['vinylSpeeds']?.[album.vinylSpeed] ?? album.vinylSpeed;
        album.mediaGrade = this.enums['grades']?.[album.mediaGrade] ?? album.mediaGrade;
        album.sleeveGrade = this.enums['grades']?.[album.sleeveGrade] ?? album.sleeveGrade;
        album.packageType = this.enums['packageTypes']?.[album.packageType] ?? album.packageType;
        album.artistGenre = this.enums['artistGenres']?.[album.artistGenre] ?? album.artistGenre;
        album.albumGenre = this.enums['albumGenres']?.[album.albumGenre] ?? album.albumGenre;
        album.albumLength = this.enums['albumLengths']?.[album.albumLength] ?? album.albumLength;
        album.albumType = this.enums['albumTypes']?.[album.albumType] ?? album.albumType;
      });

      // Update pagination service
      this.pageService.setTotalItems(this.totalItems);
      this.pageService.setCurrentData(this.data);

      this.isLoading = false;
    },
    error: (error) => {
      console.error('Error fetching data:', error);
      this.isLoading = false;
    }
  });
}



  fetchEnums() {
  const enumsUrl = `${environment.apiUrl}Constants/enums`;
  this.apiService.getData(enumsUrl).subscribe({
    next: (response: any) => {
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
    error: (error) => console.error('Error fetching enums:', error)
  });
}

  const enumsUrl = `${environment.apiUrl}Constants/enums`;
  this.apiService.getData(enumsUrl).subscribe({
    next: (response: any) => {
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
    error: (error) => console.error('Error fetching enums:', error)
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
        imageClass: this.imageClasses[clickedItem.imgFileExt],
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
    this.fetchData(this.format, this.appliedSearchQuery);
  };

  private setupSubscriptions() {
    const formatSubscription = this.sharedService.getFormatSortCriteria().subscribe(format => {
      this.format = format;
      this.currentPage = 1;
      if (this.isLoading) return;
      this.fetchData(this.format, this.appliedSearchQuery);
    });

    const searchQuerySubscription = this.sharedService.getSearchQuery().subscribe(query => {
      this.currentSearchQuery = query;
      this.appliedSearchQuery = query;
      this.currentPage = 1;
      if (this.isLoading) return;
      this.fetchData(this.format, this.appliedSearchQuery);
    });

    const mainSortSubscription = this.sharedService.getMainSortCriteria().subscribe(criteria => {
      this.mainSortCriteria = criteria;
      if (!this.isLoading) {
        this.fetchData(this.format, this.appliedSearchQuery);
      }
    });

    const alphaSortSubscription = this.sharedService.getAlphaSortCriteria().subscribe(criteria => {
      this.alphaSortCriteria = criteria;
      if (this.isLoading) return;
      this.fetchData(this.format, this.appliedSearchQuery);
    });

    const priceSortSubscription = this.sharedService.getPriceSortCriteria().subscribe(criteria => {
      this.priceSortCriteria = criteria;
      if (this.isLoading) return;
      this.fetchData(this.format, this.appliedSearchQuery);
    });

    const pageServiceSubscription = this.pageService.currentPage$.subscribe(page => {
      if (this.currentPage !== page) {
        this.currentPage = page;
        this.pageService.changePage(this.currentPage);
        if (this.isLoading) return;
        this.fetchData(this.format, this.appliedSearchQuery);
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
    this.currentSearchQuery = this.getFormattedArtistName(artistName);
    this.appliedSearchQuery = this.currentSearchQuery;
    this.currentPage = 1;
    this.fetchData(this.format, this.appliedSearchQuery);
  }  
  
  searchForRecordLabel(recordLabel: string): void {
    this.currentSearchQuery = recordLabel;
    this.appliedSearchQuery = recordLabel;
    this.currentPage = 1;
    this.fetchData(this.format, this.appliedSearchQuery);
  }

  clearSearch() {
    this.currentSearchQuery = '';
    this.appliedSearchQuery = '';
    this.currentPage = 1;
    this.fetchData(this.format);
  }
}
