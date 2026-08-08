import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guards';
import { functionalities } from './features/functionality.registry';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/login/login.page').then((module) => module.LoginPageComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    loadComponent: () =>
      import('./layout/app-shell/app-shell.component').then((module) => module.AppShellComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      ...functionalities.map((feature) => ({
        path: feature.path,
        title: feature.label,
        loadComponent: feature.loadComponent,
      })),
    ],
  },
  { path: '**', redirectTo: '' },
];
