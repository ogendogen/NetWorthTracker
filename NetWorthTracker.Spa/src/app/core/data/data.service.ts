import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../app.config';
import { NetWorthSummary } from './net-worth.models';

@Injectable({ providedIn: 'root' })
export class DataService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getSummary(): Observable<NetWorthSummary> {
    return this.http.get<NetWorthSummary>(`${this.apiBaseUrl}/data`);
  }
}
