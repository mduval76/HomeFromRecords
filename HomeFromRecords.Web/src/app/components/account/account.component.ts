import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api/api.service';
import { AuthService } from '../../services/auth/auth.service';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { jwtDecode } from 'jwt-decode';

interface DecodedToken {
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
}

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})
export class AccountComponent implements OnInit{
  isLoading: boolean = false;
  isLogin: boolean = true;
  isAdminLogin: boolean = false;
  userId: string = '';
  form!: FormGroup;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private fb: FormBuilder,
    private sb: MatSnackBar,
    private router: Router) {}

  ngOnInit(): void {
    this.createForm();
  }

  onSubmit(): void {
    console.log('onSubmit reached');
    if (this.form.valid) {
      this.isLoading = true;

      if (!this.isLogin && !this.pwdMatch()) {
        this.sb.open('Passwords do not match', 'Close', { duration: 3000 });
        return;
      }
  
      const formData = new FormData();
      Object.keys(this.form.value).forEach(key => {
        if (key !== 'ConfirmPassword') {
          formData.append(key, this.form.value[key]);
        }
      });
      
      let requestObservable: Observable<any>;
      if (this.isLogin) {
          requestObservable = this.apiService.postData(`${environment.apiUrl}User/login`, formData);
          console.log('LOGIN api');
      } else if (this.isAdminLogin) {
          formData.append('UserId', this.userId);
          requestObservable = this.apiService.putData(`${environment.apiUrl}User/update`, formData);
          console.log('UPDATE api');
      } else {
          requestObservable = this.apiService.postData(`${environment.apiUrl}User/register`, formData);
          console.log('REGISTER api');
      }
  
      requestObservable.subscribe({
        next: (response) => {
          if (this.isLogin) {
            const decodedToken = jwtDecode<DecodedToken>(response.token);
            const userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            this.authService.updateUserRole(response.token);
            console.log('decodedToken:', decodedToken);
      
            if (response.isInitialAdmin) {
              this.isAdminLogin = true;
              this.userId = response.userId;
              this.sb.open('Initial Admin Login Detected', 'Close', { duration: 3000 });
              this.switchForm();
            } else {
              if (userRole === 'Admin') {
                this.router.navigate(['/new-item-form']);
              } else {
                this.router.navigate(['/catalog']);
              }
              this.sb.open('Login successful', 'Close', { duration: 3000 });
              this.isAdminLogin = false;
            }
          } else if (this.isAdminLogin) { 
            this.isAdminLogin = false;
            this.sb.open('Update successful', 'Close', { duration: 3000 });
            this.switchForm();
          } else { 
            this.sb.open('Registration successful', 'Close', { duration: 3000 });
            this.switchForm();
          }

          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error:', error);
          let errorMessage = this.isLogin ? 'Login failed: ' : this.isAdminLogin ? 'Update failed: ' : 'Registration failed: ';
          errorMessage += error.error?.message || error.message;
          this.sb.open(errorMessage, 'Close', { duration: 3000 });
          this.isLoading = false;
        }
      });
    } else {
      this.sb.open('Form is not valid', 'Close', { duration: 3000 });
    }
  }

  private createForm() {
    const formFields = {
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[^A-Za-z0-9])/)]]
    };

    if (!this.isLogin) {
      Object.assign(formFields, {
        UserName: ['', [Validators.required]],
        FirstName: ['', [Validators.required, Validators.minLength(2)]],
        LastName: ['', [Validators.required, Validators.minLength(2)]],
        Phone: ['', [Validators.required]],
        StreetAddress: ['', [Validators.required]],
        City: ['', [Validators.required]],
        Region: ['', [Validators.required]],
        PostalCode: ['', [Validators.required]],
        Country: ['', [Validators.required]],
        ConfirmPassword: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[^A-Za-z0-9])/)]]
      });
    }
    this.form = this.fb.group(formFields);
  }

  private pwdMatch(): boolean {
    return this.form.value.Password === this.form.value.ConfirmPassword;
  }

  switchForm() {
    this.isLogin = !this.isLogin;
    this.createForm();
  }

  clearForm(): void {
    this.form.reset();
  }

  logout(): void {
    this.authService.logoutUser().subscribe({
      next: () => {
        this.sb.open('Logout successful', 'Close', { duration: 3000 });
        this.authService.clearUserRole();
        this.router.navigate(['/account']);
      },
      error: () => {
        this.sb.open('No user to log out', 'Close', { duration: 3000 });
      }
    });
  }

  onKeypressEvent(event: any){
    console.log(event.target.value);
 
 }

  private capitalizeFirstLetter(string: string): string {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }

  isFieldRequired(fieldName: string): string {
    const control = this.form.get(fieldName);
    return control?.hasError('required') ? `${this.capitalizeFirstLetter(fieldName)} is required` : '';
  }

  get usernameErrorMessage() {
    if (this.form.get('UserName')?.hasError('required')) {
      return 'Username is required';
    }
    if (this.form.get('UserName')?.hasError('username')) {
      return 'Not a valid Username';
    }
    return '';
  }

  get emailErrorMessage() {
    if (this.form.get('Email')?.hasError('required')) {
      return 'Email is required';
    }
    if (this.form.get('Email')?.hasError('email')) {
      return 'Not a valid email';
    }
    return '';
  }

  get pwdErrorMessage() {
    const pwdControl = this.form.get('Password');

    if (pwdControl?.hasError('required')) {
      return 'Password is required';
    }
    if (pwdControl?.hasError('minlength')) {
      return 'Password must be at least 6 characters long';
    }
    if (pwdControl?.hasError('pattern')) {
      return 'Password must include a non-alphanumeric character';
    }
    return '';
  }
}
