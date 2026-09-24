import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-registration-success-page',
  imports: [RouterLink, MatButtonModule, MatCardModule, TranslatePipe],
  templateUrl: './registration-success.page.html',
  styleUrl: './registration-success.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationSuccessPageComponent {}
