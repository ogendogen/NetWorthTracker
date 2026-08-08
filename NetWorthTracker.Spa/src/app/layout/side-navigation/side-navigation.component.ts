import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FunctionalityDefinition } from '../../features/functionality.registry';

@Component({
  selector: 'app-side-navigation',
  imports: [MatIconModule, MatListModule, RouterLink, RouterLinkActive],
  templateUrl: './side-navigation.component.html',
  styleUrl: './side-navigation.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SideNavigationComponent {
  readonly items = input.required<readonly FunctionalityDefinition[]>();
  readonly itemSelected = output<void>();
}
