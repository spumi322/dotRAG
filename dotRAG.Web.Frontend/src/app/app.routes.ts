import { Routes } from '@angular/router';

const TAB_KEY = 'dotrag.tab';
const VALID_TABS = new Set(['chat', 'notes', 'debug', 'settings']);
const DEFAULT_TAB = 'chat';

function lastTab(): string {
  if (typeof localStorage === 'undefined') return DEFAULT_TAB;
  const stored = localStorage.getItem(TAB_KEY);
  return stored && VALID_TABS.has(stored) ? stored : DEFAULT_TAB;
}

export const routes: Routes = [
  { path: '',         redirectTo: lastTab(), pathMatch: 'full' },
  { path: 'chat',     loadComponent: () => import('./screens/chat/chat.component') },
  { path: 'notes',    loadComponent: () => import('./screens/notes/notes.component') },
  { path: 'debug',    loadComponent: () => import('./screens/debug/debug.component') },
  { path: 'settings', loadComponent: () => import('./screens/settings/settings.component') },
  { path: '**',       redirectTo: DEFAULT_TAB },
];
