import { Routes } from '@angular/router';
import { CreateComponent } from './customer/create/create.component';
import { ViewComponent } from './customer/view/view.component';

export const routes: Routes = [
  { path: '', redirectTo: '/view', pathMatch: 'full' },
  { path: 'view', component: ViewComponent },
  { path: 'create', component: CreateComponent },
  { path: 'customer/edit/:customerId', component: CreateComponent },
  { path: '**', redirectTo: '/view' }
];
