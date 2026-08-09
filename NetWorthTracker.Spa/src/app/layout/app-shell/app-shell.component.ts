import { BreakpointObserver } from '@angular/cdk/layout';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatSidenavModule } from '@angular/material/sidenav';
import { Router, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { functionalities } from '../../features/functionality.registry';
import { SideNavigationComponent } from '../side-navigation/side-navigation.component';
import { TopBarComponent } from '../top-bar/top-bar.component';

@Component({
  selector: 'app-app-shell',
  imports: [MatSidenavModule, RouterOutlet, SideNavigationComponent, TopBarComponent],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShellComponent {
  private readonly authService = inject(AuthService);
  private readonly breakpointObserver = inject(BreakpointObserver);
  private readonly router = inject(Router);
  private readonly compactLayout = toSignal(
    this.breakpointObserver.observe('(max-width: 760px)').pipe(map((state) => state.matches)),
    { initialValue: false },
  );

  readonly functionalityItems = functionalities;
  readonly drawerOpen = signal(false);
  readonly isCompact = computed(() => this.compactLayout());
  readonly drawerMode = computed<'over' | 'side'>(() => (this.isCompact() ? 'over' : 'side'));
  readonly userName = this.authService.userName;

  toggleNavigation(): void {
    this.drawerOpen.update((isOpen) => !isOpen);
  }

  closeNavigation(): void {
    if (this.isCompact()) {
      this.drawerOpen.set(false);
    }
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }
}
