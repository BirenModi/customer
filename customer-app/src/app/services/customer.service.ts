import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Customer } from '../model/customer';

@Injectable()
export class CustomerService {
  private apiUrl = 'https://localhost:7257/api/customers';

  constructor(private http: HttpClient) {}

  getCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.apiUrl);
  }

  getCustomersByAge(age: number): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.apiUrl}/age/${age}`);
  }

  createCustomer(customer: Customer): Observable<any> {
    return this.http.post<any>(this.apiUrl, customer);
  }

  getCustomerById(id: string): Observable<any> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  updateCustomer(customer: Customer): Observable<any> {
    return this.http.patch<any>(
      `${this.apiUrl}/${customer.customerId}`,
      customer
    );
  }
}
