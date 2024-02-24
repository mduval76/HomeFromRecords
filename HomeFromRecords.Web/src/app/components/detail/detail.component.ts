import { Component, OnInit, OnDestroy, Inject, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../services/api/api.service';
import { AuthService } from '../../services/auth/auth.service';
import { EnumService } from '../../services/enum/enum.service';
import { FormatService } from '../../services/format/format.service';
import { OrderService, CartItem } from '../../services/order/order.service';
import { PageService } from '../../services/page/page.service';
import { SharedService } from '../../services/shared/shared.service';
import { Subscription } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './detail.component.html',
  styleUrl: './detail.component.scss'
})
export class DetailComponent implements OnInit, OnDestroy {
  enums: any = {};
  formattedArtistName: string = '';
  formattedDetails: string[] = [];
  formattedSubDetails: string[] = [];
  includesSubstringFormatting: boolean = false;
  isAdmin: boolean = false;
  nameLength: number = 0;

  originalValues: { [key: string]: any } = {};
  activeFields: { [key: string]: boolean } = {};

  form: FormGroup = new FormGroup({
    ArtistName: new FormControl(''),
    Title: new FormControl(''),
    Price: new FormControl('', Validators.min(0)),
    Quantity: new FormControl('', Validators.min(0)),
    RecordLabelName: new FormControl(''),
    Country: new FormControl(''),
    ReleaseYear: new FormControl(''),
    CatalogNumber: new FormControl(''),
    MatrixNumber: new FormControl(''),
    ImgFileExt : new FormControl(''),
    Format: new FormControl(''),
    VinylSpeed: new FormControl(''),
    SubFormat: new FormControl(''),
    PackageType: new FormControl(''),
    AlbumType: new FormControl(''),
    AlbumLength: new FormControl(''),
    AlbumGenre: new FormControl(''),
    ArtistGenre: new FormControl(''),
    MediaGrade: new FormControl(''),
    SleeveGrade: new FormControl(''),
    Details: new FormControl('')
  });

  @ViewChild('header') header!: ElementRef;
  @ViewChild('detail') detail!: ElementRef;

  private subscriptions = new Subscription();
  private dataReady = false;
  private currentHeaderHeight: number = 0;

  constructor(
    public dialogRef: MatDialogRef<DetailComponent>,
    private apiService: ApiService,
    public authService: AuthService,
    private enumService: EnumService,
    private formatService: FormatService,
    private orderService: OrderService,
    private pageService: PageService,
    private sharedService: SharedService,
    private changeDetectorRef: ChangeDetectorRef,
    @Inject(MAT_DIALOG_DATA) public data: { 
      selectedItem: any;
      items: any[];
      currentIndex: number,
      globalIndex: number,
      totalItems: number,
      itemsPerPage: number,
      currentPage: number
    }
  ) {
    const dataSubscription = this.pageService.currentData$.subscribe(updatedData => {
      this.data.items = updatedData;
      this.dataReady = true;
      this.updateItem(this.data.globalIndex % this.data.itemsPerPage);
    });

    this.subscriptions.add(dataSubscription);
  }

  ngOnInit() {
    const authSub = this.authService.userRoleObservable.subscribe(role => {
      this.isAdmin = role === 'Admin';
    });

    const enumSub = this.enumService.fetchEnums().subscribe(enums => {
      this.enums = enums;
    });

    this.subscriptions.add(authSub);
    this.subscriptions.add(enumSub);
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  goToNext(): void {
    if (this.data.globalIndex < this.data.totalItems - 1) {
      this.data.globalIndex++;
      this.clearForm();
      this.updateCurrentItemAndView();
    }
  }

  // Pagination functions
  goToPrevious(): void {
    if (this.data.globalIndex > 0) {
      this.data.globalIndex--;
      this.clearForm();
      this.updateCurrentItemAndView();
    }
  }

  updateCurrentItemAndView(): void {
    const newPage = Math.floor(this.data.globalIndex / this.data.itemsPerPage) + 1;
    const newLocalIndex = this.data.globalIndex % this.data.itemsPerPage;

    if (newPage !== this.data.currentPage) {
      this.pageService.changePage(newPage);
      this.dataReady = false;
      this.data.currentPage = newPage;
    } else {
      this.updateItem(newLocalIndex);
      this.detail.nativeElement.style.paddingTop = `${this.currentHeaderHeight}px`;
    }
  }

  // Form functions
  updateCurrentField(field: string): void {
    this.toggleActiveField(field);
  }

  hasActiveFields(): boolean {
    return Object.keys(this.activeFields).length > 0;
  }

  toggleActiveField(field: string): void {
    if (this.activeFields[field]) {
      delete this.activeFields[field];
    } else {
      this.activeFields[field] = true;
    }

    this.activeFields = { ...this.activeFields };
    this.changeDetectorRef.detectChanges();
  }

  initializeFormWithData(selectedItem: any): void {
    Object.keys(selectedItem).forEach(key => {
      const control = new FormControl(selectedItem[key]);
      this.form.addControl(key, control);
      this.originalValues[key] = selectedItem[key];
    });
  }

  subscribeToFormChanges(): void {
    Object.keys(this.form.controls).forEach(fieldName => {
      const control = this.form.get(fieldName);
      control!.valueChanges.subscribe(newValue => {
        if (newValue !== this.originalValues[fieldName]) {
          this.activeFields[fieldName] = true;
        } else {
          delete this.activeFields[fieldName];
        }
      });
    });
  }

  // Helper functions
  private updateItem(newLocalIndex: number): void {
    if (this.dataReady) { 
      this.data.currentIndex = newLocalIndex;
      this.data.selectedItem = this.data.items[this.data.currentIndex];
      this.formattedArtistName = this.formatService.formatArtistName(this.data.selectedItem.artistName);
      this.formattedDetails = this.formatService.formatDetails(this.data.selectedItem.details);
      this.updateFontSizeClass();
    }
  }

  updateFontSizeClass(): string {
    this.nameLength = this.data.selectedItem.artistName.length;
    this.adjustPaddingTop();

    if (this.nameLength < 25) {
      return 'large-font';
    } else if (this.nameLength < 50) {
      return 'medium-font';
    } else {
      return 'small-font';
    }
  }

  private adjustPaddingTop() {
    if (this.header && this.header.nativeElement) {
      this.currentHeaderHeight = this.header.nativeElement.offsetHeight;
      this.detail.nativeElement.style.paddingTop = `${this.currentHeaderHeight}px`;
    }
  }

  closeDialog(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formData = new FormData();

      Object.keys(this.form.value).forEach(key => {
        let value = this.form.value[key];
        formData.append(key, value);
      });

      var albumId = this.data.selectedItem.albumId;
      this.apiService.putData(`${environment.apiUrl}Album/update?albumId=${albumId}`, formData).subscribe({
        next: (response) => {
        console.log('Response from server:', response);
        this.clearForm();
        this.sharedService.notifyDataRefreshNeeded();
      },
      error: (error) => {
        console.error('Error:', error);
      }
    });
      console.log('Submitting form', formData);
    }
    else {
      console.error('Form is not valid');
    }
  }

  clearForm(): void {
    Object.keys(this.form.controls).forEach(key => {
      this.form.controls[key].reset('');
    });
    this.activeFields = {};
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

  searchForArtist(artistName: string): void {
    this.sharedService.setSearchQuery(artistName);
    this.closeDialog();
  }
}
