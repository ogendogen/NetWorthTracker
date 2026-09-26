import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { TranslatePipe } from '@ngx-translate/core';
import { Router, RouterLink } from '@angular/router';
import { take, timer } from 'rxjs';

@Component({
  selector: 'app-email-confirmed-page',
  imports: [RouterLink, MatButtonModule, MatCardModule, TranslatePipe],
  templateUrl: './email-confirmed.page.html',
  styleUrl: './email-confirmed.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmailConfirmedPageComponent {
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  readonly secondsRemaining = signal(10);

  constructor() {
    timer(1000, 1000)
      .pipe(take(10), takeUntilDestroyed(this.destroyRef))
      .subscribe((elapsedSeconds) => {
        const secondsRemaining = 9 - elapsedSeconds;
        this.secondsRemaining.set(secondsRemaining);

        if (secondsRemaining === 0) {
          void this.router.navigateByUrl('/login');
        }
      });
  }
}
