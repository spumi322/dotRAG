import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';

import { ChatRequest, ChatResponse, ChunkDto, HistoryMessage, Role } from '../../core/api.types';
import { ToastService } from '../../core/toast.service';

export interface Message {
  readonly id: number;
  readonly role: Role;
  readonly content: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly http   = inject(HttpClient);
  private readonly toasts = inject(ToastService);
  private nextId = 1;

  // Singleton state — survives /chat <-> /debug navigation.
  readonly messages       = signal<readonly Message[]>([]);
  readonly history        = signal<readonly HistoryMessage[]>([]);
  readonly lastChunks     = signal<readonly ChunkDto[]>([]);
  readonly sending        = signal(false);
  readonly includeHistory = signal(true);

  readonly canSend = computed(() => !this.sending());

  send(question: string) {
    const trimmed = question.trim();
    if (!trimmed || this.sending()) return;

    this.append('user', trimmed);
    this.sending.set(true);

    const req: ChatRequest = {
      question: trimmed,
      ...(this.includeHistory() && this.history().length > 0
        ? { history: [...this.history()] }
        : {}),
    };

    this.http.post<ChatResponse>('/api/chat', req).subscribe({
      next: resp => {
        const answer = resp?.answer ?? '';
        if (!answer) {
          this.toasts.error('Empty response from /api/chat. Check backend logs.');
          this.sending.set(false);
          return;
        }
        this.append('assistant', answer);
        this.lastChunks.set(resp.chunks ?? []);
        this.history.update(h => [
          ...h,
          { role: 'user',      content: trimmed },
          { role: 'assistant', content: answer  },
        ]);
        this.sending.set(false);
      },
      error: err => {
        const detail = err?.status === 503
          ? 'Service warming up — try again in a moment.'
          : err?.status === 0
            ? 'Cannot reach backend — is dotRAG.API running on :5081?'
            : (err?.error?.title ?? err?.message ?? `Request failed (${err?.status ?? '?'}).`);
        this.toasts.error(detail);
        this.sending.set(false);
      },
    });
  }

  clear() {
    this.messages.set([]);
    this.history.set([]);
    this.lastChunks.set([]);
  }

  private append(role: Role, content: string) {
    this.messages.update(m => [...m, { id: this.nextId++, role, content }]);
  }
}
