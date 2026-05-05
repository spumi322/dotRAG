import {
  ApplicationConfig,
  inject,
  provideBrowserGlobalErrorListeners,
  provideEnvironmentInitializer,
} from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NavigationEnd, Router, provideRouter } from '@angular/router';
import { filter } from 'rxjs/operators';

import { routes } from './app.routes';
import { readinessInterceptor } from './core/readiness.interceptor';
import { HealthService } from './core/health.service';

const TAB_KEY = 'dotrag.tab';
const VALID_TABS = new Set(['chat', 'notes', 'debug', 'settings']);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([readinessInterceptor])),
    provideEnvironmentInitializer(() => {
      const router = inject(Router);
      router.events
        .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
        .subscribe(e => {
          const segment = e.urlAfterRedirects.split('?')[0].split('/')[1] ?? '';
          if (VALID_TABS.has(segment)) {
            localStorage.setItem(TAB_KEY, segment);
          }
        });

      inject(HealthService).start();
    }),
  ],
};
