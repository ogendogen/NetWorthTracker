import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-liabilities-page',
  imports: [MatCardModule],
  template: `
    <section aria-labelledby="liabilities-title">
      <h1 id="liabilities-title">Liabilities</h1>
      <mat-card
        ><mat-card-content>Liability tracking will be available here.</mat-card-content></mat-card
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
export class LiabilitiesPageComponent {}
