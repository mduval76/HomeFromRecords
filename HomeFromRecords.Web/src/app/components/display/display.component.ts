import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription, BehaviorSubject } from 'rxjs';
import { distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ApiService } from '../../services/api/api.service';
import { FormatService } from '../../services/format/format.service';
import { OrderService, CartItem } from '../../services/order/order.service';
import { PageService } from '../../services/page/page.service';
import { SharedService } from '../../services/shared/shared.service';
import { DetailComponent } from '../detail/detail.component';
import { PaginationComponent } from '../pagination/pagination.component';
import { environment } from '../../../environments/environment.development';
import { AlbumDto } from '../../models/album.model';

type EnumDictionary = { [key: number]: string };

interface EnumItem { name: string; value: number; }
interface DisplayState {
  format: number;
  searchQuery: string;
  mainSort: string;
  alphaSort: 'ascending' | 'descending';
  priceSort: 'none' | 'ascending' | 'descending';
  currentPage: number;
  itemsPerPage: number;
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
  defaultImg = '../../../assets/images/defaults/default.webp';
  enums: Record<string, EnumDictionary> = {};
  data: any[] = [];
  displayedData: any[] = [];
  totalItems = 0;
  isLoading = false;
  imageClasses: Record<string, string> = {};

  private subscriptions = new Subscription();

  public state$ = new BehaviorSubject<DisplayState>({
    format: 666,
    searchQuery: '',
    mainSort: 'Artist',
    alphaSort: 'ascending',
    priceSort: 'none',
    currentPage: 1,
    itemsPerPage: 12
  });

  constructor(
    private apiService: ApiService,
    private formatService: FormatService,
    private orderService: OrderService,
    private pageService: PageService,
    private sharedService: SharedService,
    public dialog: MatDialog
    ) {}

  ngOnInit() {
    this.fetchEnums();
    this.setupSubscriptions();

    this.subscriptions.add(
      this.state$
        .pipe(
          distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
          switchMap(state => {
            this.isLoading = true;
            return this.apiService.getData(this.buildApiUrl(state));
          })
        )
        .subscribe({
          next: (response: { items: AlbumDto[]; totalItems: number }) => {
            if (!response || !Array.isArray(response.items)) {
              console.error('Invalid API response', response);
              this.isLoading = false;
              return;
            }

            this.data = response.items.map((album: AlbumDto) => this.mapEnums(album));
            this.data.forEach(album => this.updateImageClass(album.imgFileExt));
            this.totalItems = response.totalItems;

            this.pageService.setTotalItems(this.totalItems);
            this.pageService.setCurrentData(this.data);

            this.isLoading = false;
          },
          error: error => {
            console.error('Error fetching data:', error);
            this.isLoading = false;
          }
        })
    );
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  private buildApiUrl(state: DisplayState) {
    const baseUrl = `${environment.apiUrl}Album/paged`;
    const params = new URLSearchParams();

    params.set('page', state.currentPage.toString());
    params.set('itemsPerPage', state.itemsPerPage.toString());

    if (state.searchQuery) params.set('searchQuery', state.searchQuery);
    if (state.format !== 666) params.set('mainFormat', state.format.toString());

    // Sorting
    if (state.priceSort !== 'none') {
      params.set('sortBy', 'price');
      params.set('ascending', state.priceSort === 'ascending' ? 'true' : 'false');
    } else {
      params.set('sortBy', this.mapSortField(state.mainSort));
      params.set('ascending', state.alphaSort === 'ascending' ? 'true' : 'false');
    }

    return `${baseUrl}?${params.toString()}`;
  }

  private mapEnums(album: any) {
    return {
      ...album,
      format: this.enums['mainFormats']?.[album.format] ?? album.format,
      subFormat: this.enums['subFormats']?.[album.subFormat] ?? album.subFormat,
      vinylSpeed: this.enums['vinylSpeeds']?.[album.vinylSpeed] ?? album.vinylSpeed,
      mediaGrade: this.enums['grades']?.[album.mediaGrade] ?? album.mediaGrade,
      sleeveGrade: this.enums['grades']?.[album.sleeveGrade] ?? album.sleeveGrade,
      packageType: this.enums['packageTypes']?.[album.packageType] ?? album.packageType,
      artistGenre: this.enums['artistGenres']?.[album.artistGenre] ?? album.artistGenre,
      albumGenre: this.enums['albumGenres']?.[album.albumGenre] ?? album.albumGenre,
      albumLength: this.enums['albumLengths']?.[album.albumLength] ?? album.albumLength,
      albumType: this.enums['albumTypes']?.[album.albumType] ?? album.albumType
    };
  }

  getIconUrl(format: string): string {
    const formatIcons: Record<string, string> = {
      'Vinyl': '../../../assets/images/icons/LP.png',
      'CD': '../../../assets/images/icons/CD.png',
      'Cassette': '../../../assets/images/icons/K7.png'
    };

    return `url('${formatIcons[format]}')`;
  }

  private updateState(partial: Partial<DisplayState>) {
    this.state$.next({ ...this.state$.getValue(), ...partial });
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

  private updateImageClass(imageUrl: string) {
    const img = new Image();
    img.onload = () => {
      if (img.width === 500 && img.height === 500) this.imageClasses[imageUrl] = 'square-format';
      else if (img.height === 500 && img.width < 500) this.imageClasses[imageUrl] = 'tall-format';
      else this.imageClasses[imageUrl] = 'wide-format';
    };
    img.onerror = () => (this.imageClasses[imageUrl] = 'default-format');
    img.src = imageUrl;
  }

  
  handleImageError(event: Event, defaultImageUrl: string) {
    (event.target as HTMLImageElement).src = defaultImageUrl;
  }

  getFormattedArtistName(str: string) {
    return this.formatService.formatArtistName(str);
  }

  get isEmpty(): boolean {
    return this.data.length === 0;
  }

  addToCart(album: any) {
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

  openDialog(album: any, itemIndex: number) {
    const state = this.state$.getValue();
    const globalIndex = (state.currentPage - 1) * state.itemsPerPage + itemIndex;

    const dialogRef = this.dialog.open(DetailComponent, {
      panelClass: 'border-dialog',
      data: {
        selectedItem: album,
        items: this.displayedData,
        imageClass: this.imageClasses[album.imgFileExt],
        currentIndex: itemIndex,
        globalIndex,
        totalItems: this.totalItems,
        itemsPerPage: state.itemsPerPage,
        currentPage: state.currentPage
      }
    });

    dialogRef.afterClosed().subscribe(result => console.log(`Dialog result: ${result}`));
  }

  private setupSubscriptions() {
    this.subscriptions.add(
      this.sharedService.getFormatSortCriteria().subscribe(format =>
        this.updateState({ format, currentPage: 1 })
      )
    );

    this.subscriptions.add(
      this.sharedService.getSearchQuery().subscribe(query =>
        this.updateState({ searchQuery: query, currentPage: 1 })
      )
    );

    this.subscriptions.add(
      this.sharedService.getMainSortCriteria().subscribe(mainSort =>
        this.updateState({ mainSort, currentPage: 1 })
      )
    );

    this.subscriptions.add(
      this.sharedService.getAlphaSortCriteria().subscribe(alphaSort => 
        this.updateState({ alphaSort: alphaSort as 'ascending' | 'descending' })
      )
    );

    this.subscriptions.add(
      this.sharedService.getPriceSortCriteria().subscribe(priceSort =>
        this.updateState({ priceSort: priceSort as 'none' | 'ascending' | 'descending' })
      )
    );

    this.subscriptions.add(
      this.pageService.currentPage$.subscribe(currentPage =>
        this.updateState({ currentPage })
      )
    );

    this.subscriptions.add(
      this.sharedService.getRefreshNeeded().subscribe(needed => {
        if (needed) this.updateState({ currentPage: 1 });
      })
    );
  }

  searchForArtist(artistName: string) {
    this.updateState({ searchQuery: this.formatService.formatArtistName(artistName), currentPage: 1 });
  }

  searchForRecordLabel(recordLabel: string) {
    this.updateState({ searchQuery: recordLabel, currentPage: 1 });
  }

  clearSearch() {
    this.updateState({ searchQuery: '', currentPage: 1 });
  }

  private mapSortField(mainSort: string): string {
    switch (mainSort) {
      case 'Artist': return 'artistName';
      case 'Album': return 'title';
      default: return 'artistName';
    }
  }
}
