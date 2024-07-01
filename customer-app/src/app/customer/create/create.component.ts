import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { Customer } from '../../model/customer';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss'],
})
export class CreateComponent implements OnInit {
  customerForm: FormGroup;
  customerId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService
  ) {
    this.customerForm = this.fb.group({
      fullName: ['', Validators.required],
      dateOfBirth: [
        '',
        [Validators.required, this.dateOfBirthValidator.bind(this)],
      ],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.customerId = params['customerId'];
      if (this.customerId) {
        this.customerService
          .getCustomerById(this.customerId)
          .subscribe((customer) => {
            if (customer) {
              this.customerForm.patchValue(customer);
            } else {
              this.router.navigate(['/view']);
            }
          },
          (error) => {
            console.error('Error getting customer:', error);
            alert('Error getting customer');
            this.router.navigate(['/view']);
          });
      }
    });
  }

  dateOfBirthValidator(control: any) {
    const dateString = control.value;
    if (!dateString) {
      return null;
    }

    const selectedDate = new Date(dateString);

    if (isNaN(selectedDate.getTime())) {
      return { dateOfBirthInvalid: true };
    }

    const today = new Date();

    selectedDate.setHours(0, 0, 0, 0);
    today.setHours(0, 0, 0, 0);

    if (selectedDate > today) {
      return { dateOfBirthInvalid: true };
    }

    return null;
  }

  saveCustomer() {
    if (this.customerForm.valid) {
      const formData = this.customerForm.value as Customer;
      formData.dateOfBirth = this.formatDateToISO(
        new Date(formData.dateOfBirth)
      );

      if (this.customerId) {
        // Update existing customer
        formData.customerId = this.customerId;
        this.customerService.updateCustomer(formData).subscribe(
          () => {
            console.log('Customer updated successfully');
            alert('Customer updated successfully');
            this.router.navigate(['/view']);
          },
          (error) => {
            console.error('Error updating customer:', error);
            alert('Error updating customer');
          }
        );
      } else {
        // Create new customer
        this.customerService.createCustomer(formData).subscribe(
          () => {
            console.log('Customer created successfully');
            alert('Customer created successfully');
            this.router.navigate(['/view']);
          },
          (error) => {
            console.error('Error creating customer:', error);
            alert('Error creating customer');
          }
        );
      }
    } else {
      this.customerForm.markAllAsTouched();
    }
  }

  private formatDateToISO(originalDateString: Date): string {
    let date = new Date(originalDateString);
    let dd: any = date.getDate();
    let mm: any = date.getMonth() + 1; // January is 0!

    let yyyy: any = date.getFullYear();
    if (dd < 10) {
      dd = '0' + dd;
    }
    if (mm < 10) {
      mm = '0' + mm;
    }

    return yyyy + '-' + mm + '-' + dd;
  }
}
