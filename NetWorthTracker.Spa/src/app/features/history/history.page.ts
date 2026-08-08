import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-history-page',
  imports: [MatCardModule],
  template: `
    <section aria-labelledby="history-title">
      <h1 id="history-title">History</h1>
      <mat-card
        ><mat-card-content>Net worth history will be available here.</mat-card-content></mat-card
      >
    </section>
  `,
  styles: [
    `
      h1 {
        margin-top: 0;
      }
      mat-card {
        max-width: 44rem;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HistoryPageComponent {}
