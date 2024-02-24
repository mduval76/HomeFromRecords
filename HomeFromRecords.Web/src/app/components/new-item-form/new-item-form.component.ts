import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ApiService } from '../../services/api/api.service';
import { EnumService } from '../../services/enum/enum.service';
import { Subscription } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'new-item-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './new-item-form.component.html',
  styleUrl: './new-item-form.component.scss'
})
export class NewItemFormComponent implements OnInit, OnDestroy {
  enums: any = {};
  selectedFile: File | null = null;

  private enumSubscription: Subscription | null = null;

  form: FormGroup = new FormGroup({
    ArtistName: new FormControl('', [Validators.required]),
    Title: new FormControl('', [Validators.required]),
    Price: new FormControl('', [Validators.required, Validators.min(0)]),
    Quantity: new FormControl('', [Validators.required, Validators.min(0)]),
    RecordLabelName: new FormControl('', [Validators.required]),
    Country: new FormControl('', [Validators.required]),
    ReleaseYear: new FormControl('', [Validators.required]),
    CatalogNumber: new FormControl('', [Validators.required]),
    MatrixNumber: new FormControl(''),
    ImgFileExt : new FormControl(''),
    Format: new FormControl('', [Validators.required]),
    VinylSpeed: new FormControl(''),
    SubFormat: new FormControl(''),
    PackageType: new FormControl(''),
    AlbumType: new FormControl(''),
    AlbumLength: new FormControl(''),
    AlbumGenre: new FormControl('', [Validators.required]),
    ArtistGenre: new FormControl('', [Validators.required]),
    MediaGrade: new FormControl('', [Validators.required]),
    SleeveGrade: new FormControl('', [Validators.required]),
    Details: new FormControl('', [Validators.required])
  });
  
  constructor(private apiService: ApiService, private enumService: EnumService, private router: Router) {}

  ngOnInit() {
    this.enumSubscription = this.enumService.fetchEnums().subscribe(enums => {
      this.enums = enums;
    });
    this.initializeFormValueSubscriptions();
  }

  ngOnDestroy() {
    if (this.enumSubscription) {
      this.enumSubscription.unsubscribe();
    }
  }

  initializeFormValueSubscriptions() {
    this.form.get('Format')?.valueChanges.subscribe(() => {
      if (!this.isVinylSelected) {
        this.form.get('VinylSpeed')?.setValue(4);
      } else {
        this.form.get('VinylSpeed')?.reset();
      }
    });
  }

  get isVinylSelected(): boolean {
    return this.form.get('Format')?.value === 0;
  }

  getMainFormatStyle(): any {
    const mainFormatControl = this.form.get('Format');

    if (mainFormatControl && mainFormatControl.value !== null) {
      return mainFormatControl.value === 0 ? {'width': '48%'} : {'width': '100%'};
    } else {
      return {'width': '100%'};
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formData = new FormData();

      if (this.selectedFile) {
        formData.append('file', this.selectedFile, this.selectedFile.name);
      }

      Object.keys(this.form.value).forEach(key => {
        let value = this.form.value[key];
  
        if (key !== 'Price') {
          if (['Quantity', 'MediaGrade', 'SleeveGrade', 'Format', 'VinylSpeed', 'SubFormat', 
                'PackageType', 'AlbumGenre', 'ArtistGenre', 'AlbumLength', 'AlbumType'].includes(key)) {
            value = parseInt(value, 10);
          }
        }
  
        formData.append(key, value);
      });

      // Debugging FormData
      for (let [key, value] of (formData as any).entries()) {
        console.log(`${key}: ${value}`);
      }

      this.apiService.postData(`${environment.apiUrl}Album/add`, formData).subscribe({
        next: (response) => {
          console.log('Response from server:', response);
          this.router.navigate(['/home']);
        },
        error: (error) => {
          console.error('Error:', error);
        }
      });
    } 
    else {
      console.error('Form is not valid');
    }
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
  
    if (file) {
      console.log("Selected file name:", file.name);
      this.selectedFile = file;
      this.form.patchValue({
        ImgFileExt: file.name
      });
    }
  }

  clearForm(): void {
    this.form.reset();
  }
}