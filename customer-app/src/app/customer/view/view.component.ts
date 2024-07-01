import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../model/customer';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  customers: Customer[] = [];
  displayedColumns: string[] = ['fullName', 'dateOfBirth', 'actions'];
  ageFilter: number | undefined;

  constructor(
    private router: Router,
    private customerService: CustomerService
  ) { }

  ngOnInit(): void {
    this.fetchCustomers();
  }

  fetchCustomers() {
    if (this.ageFilter !== undefined && this.ageFilter !== null) {
      this.customerService.getCustomersByAge(this.ageFilter)
        .subscribe(customers => {
          this.customers = customers;
        });
    } else {
      this.customerService.getCustomers()
        .subscribe(customers => {
          this.customers = customers;
        });
    }
  }

  applyAgeFilter() {
    this.fetchCustomers();
  }

  editCustomer(customer: Customer) {
    this.router.navigate(['/customer', 'edit', customer.customerId]);
  }

  validateNumber(event: KeyboardEvent) {
    const charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      event.preventDefault();
    }
  }
}