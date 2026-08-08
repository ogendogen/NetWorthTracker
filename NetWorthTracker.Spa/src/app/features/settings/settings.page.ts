import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-settings-page',
  imports: [MatCardModule],
  template: `
    <section aria-labelledby="settings-title">
      <h1 id="settings-title">Settings</h1>
      <mat-card
        ><mat-card-content>Application settings will be available here.</mat-card-content></mat-card
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
export class SettingsPageComponent {}
