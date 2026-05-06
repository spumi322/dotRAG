import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';

import { NotesFileDetail, NotesIndex } from '../../core/api.types';
import { ToastService } from '../../core/toast.service';

@Injectable({ providedIn: 'root' })
export class NotesService {
  private readonly http   = inject(HttpClient);
  private readonly toasts = inject(ToastService);

  readonly index               = signal<NotesIndex | null>(null);
  readonly selectedFile        = signal<NotesFileDetail | null>(null);
  readonly selectedChunkIndex  = signal<number | null>(null);
  readonly loading             = signal(false);
  readonly loadingFile         = signal(false);

  loadIndex() {
    if (this.loading()) return;
    this.loading.set(true);
    this.http.get<NotesIndex>('/api/notes/files').subscribe({
      next: idx => {
        this.index.set(idx);
        this.loading.set(false);
      },
      error: err => {
        this.toasts.error(this.describeError(err, '/api/notes/files'));
        this.loading.set(false);
      },
    });
  }

  selectFile(relativePath: string) {
    this.loadingFile.set(true);
    this.selectedChunkIndex.set(null);
    // Catch-all route token preserves slashes — encode each segment so the
    // file portion (which can include spaces/parens) round-trips safely.
    const url = '/api/notes/files/' + relativePath.split('/').map(encodeURIComponent).join('/');
    this.http.get<NotesFileDetail>(url).subscribe({
      next: file => {
        this.selectedFile.set(file);
        this.loadingFile.set(false);
      },
      error: err => {
        this.toasts.error(this.describeError(err, relativePath));
        this.loadingFile.set(false);
      },
    });
  }

  selectChunk(index: number) {
    this.selectedChunkIndex.set(index);
  }

  reindex() {
    this.toasts.info('Re-index not implemented yet.');
  }

  private describeError(err: HttpErrorResponse, label: string): string {
    if (err?.status === 503) return 'Service warming up — try again in a moment.';
    if (err?.status === 0)   return 'Cannot reach backend — is dotRAG.API running on :5081?';
    if (err?.status === 404) return `Not found: ${label}`;
    return err?.error?.title ?? err?.message ?? `Request failed (${err?.status ?? '?'}).`;
  }
}
