import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-email-confirmation-failed-page',
  imports: [RouterLink, MatButtonModule, MatCardModule, TranslatePipe],
  templateUrl: './email-confirmation-failed.page.html',
  styleUrl: './email-confirmation-failed.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmailConfirmationFailedPageComponent {}
