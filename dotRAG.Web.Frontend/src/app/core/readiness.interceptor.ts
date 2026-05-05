import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { ToastService } from './toast.service';

// Surfaces 503s on /api/* as a "service warming up" toast.
// Other errors pass through untouched — components handle their own error UX.
export const readinessInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith('/api/')) return next(req);

  const toasts = inject(ToastService);
  return next(req).pipe(
    catchError(err => {
      if (err?.status === 503) {
        toasts.info('Notes ingestion in progress — please retry shortly.');
      }
      return throwError(() => err);
    })
  );
};
