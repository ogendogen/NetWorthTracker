import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-assets-page',
  imports: [MatCardModule],
  template: `
    <section aria-labelledby="assets-title">
      <h1 id="assets-title">Assets</h1>
      <mat-card
        ><mat-card-content>Asset tracking will be available here.</mat-card-content></mat-card
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
export class AssetsPageComponent {}
