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
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/register/register.page').then((module) => module.RegisterPageComponent),
  },
  {
    path: 'registration-success',
    loadComponent: () =>
      import('./features/registration-success/registration-success.page').then(
        (module) => module.RegistrationSuccessPageComponent,
      ),
  },
  {
    path: 'email-confirmed',
    loadComponent: () =>
      import('./features/email-confirmed/email-confirmed.page').then(
        (module) => module.EmailConfirmedPageComponent,
      ),
  },
  {
    path: 'email-confirmation-failed',
    loadComponent: () =>
      import('./features/email-confirmation-failed/email-confirmation-failed.page').then(
        (module) => module.EmailConfirmationFailedPageComponent,
      ),
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/forgot-password/forgot-password.page').then(
        (module) => module.ForgotPasswordPageComponent,
      ),
  },
  {
    path: 'forgot-password-sent',
    loadComponent: () =>
      import('./features/forgot-password-sent/forgot-password-sent.page').then(
        (module) => module.ForgotPasswordSentPageComponent,
      ),
  },
  {
    path: 'reset-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/reset-password/reset-password.page').then(
        (module) => module.ResetPasswordPageComponent,
      ),
  },
  {
    path: 'password-reset-success',
    loadComponent: () =>
      import('./features/password-reset-success/password-reset-success.page').then(
        (module) => module.PasswordResetSuccessPageComponent,
      ),
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
