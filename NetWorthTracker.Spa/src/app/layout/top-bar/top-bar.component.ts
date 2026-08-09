import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-top-bar',
  imports: [MatButtonModule, MatIconModule, MatToolbarModule, MatTooltipModule],
  templateUrl: './top-bar.component.html',
  styleUrl: './top-bar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopBarComponent {
  readonly showMenuButton = input(false);
  readonly userName = input<string | null>(null);
  readonly menuToggle = output<void>();
  readonly logout = output<void>();
}
