import { Routes } from '@angular/router';
import { AboutComponent } from './components/about/about.component';
import { CartComponent } from './components/cart/cart.component';
import { DisplayComponent } from './components/display/display.component';
import { HomeComponent } from './components/home/home.component';
import { NewItemFormComponent } from './components/new-item-form/new-item-form.component';
import { AccountComponent } from './components/account/account.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'about', component: AboutComponent },
    { path: 'account', component: AccountComponent },
    { path: 'cart', component: CartComponent },
    { path: 'catalog', component: DisplayComponent },
    { path: 'home', component: HomeComponent },
    { path: 'new-item-form', component: NewItemFormComponent }
];
