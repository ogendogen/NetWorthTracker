import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DataService } from '../../core/data/data.service';
import { NetWorthSummary } from '../../core/data/net-worth.models';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    CurrencyPipe,
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPageComponent {
  private readonly dataService = inject(DataService);

  readonly isLoading = signal(true);
  readonly hasError = signal(false);
  readonly summary = signal<NetWorthSummary | null>(null);

  constructor() {
    this.loadSummary();
  }

  loadSummary(): void {
    this.isLoading.set(true);
    this.hasError.set(false);
    this.dataService.getSummary().subscribe({
      next: (summary) => this.summary.set(summary),
      error: () => {
        this.hasError.set(true);
        this.isLoading.set(false);
      },
      complete: () => this.isLoading.set(false),
    });
  }
}
