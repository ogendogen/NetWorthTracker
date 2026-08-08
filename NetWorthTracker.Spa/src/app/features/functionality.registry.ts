import { Type } from '@angular/core';

export interface FunctionalityDefinition {
  path: string;
  label: string;
  icon: string;
  loadComponent: () => Promise<Type<unknown>>;
}

export const functionalities: readonly FunctionalityDefinition[] = [
  {
    path: 'dashboard',
    label: 'Dashboard',
    icon: 'space_dashboard',
    loadComponent: () =>
      import('./dashboard/dashboard.page').then((module) => module.DashboardPageComponent),
  },
  {
    path: 'assets',
    label: 'Assets',
    icon: 'account_balance_wallet',
    loadComponent: () =>
      import('./assets/assets.page').then((module) => module.AssetsPageComponent),
  },
  {
    path: 'liabilities',
    label: 'Liabilities',
    icon: 'credit_card',
    loadComponent: () =>
      import('./liabilities/liabilities.page').then((module) => module.LiabilitiesPageComponent),
  },
  {
    path: 'history',
    label: 'History',
    icon: 'timeline',
    loadComponent: () =>
      import('./history/history.page').then((module) => module.HistoryPageComponent),
  },
  {
    path: 'settings',
    label: 'Settings',
    icon: 'settings',
    loadComponent: () =>
      import('./settings/settings.page').then((module) => module.SettingsPageComponent),
  },
];
