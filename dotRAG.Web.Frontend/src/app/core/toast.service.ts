import { Injectable, signal } from '@angular/core';

export type ToastKind = 'error' | 'success' | 'info';

export interface Toast {
  readonly id: number;
  readonly kind: ToastKind;
  readonly message: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private static readonly DEFAULT_TTL_MS = 4000;
  private nextId = 1;

  readonly toasts = signal<readonly Toast[]>([]);

  error(message: string)   { this.show('error',   message); }
  success(message: string) { this.show('success', message); }
  info(message: string)    { this.show('info',    message); }

  dismiss(id: number) {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private show(kind: ToastKind, message: string) {
    const id = this.nextId++;
    this.toasts.update(list => [...list, { id, kind, message }]);
    setTimeout(() => this.dismiss(id), ToastService.DEFAULT_TTL_MS);
  }
}
