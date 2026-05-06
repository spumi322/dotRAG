import {
  AfterViewChecked,
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  TemplateRef,
  ViewChild,
  effect,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { MarkdownPipe } from '../../core/markdown.pipe';
import { RightPanelService } from '../../core/right-panel.service';
import { ChatService } from './chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, MarkdownPipe],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
})
export default class ChatComponent implements AfterViewChecked, AfterViewInit, OnDestroy {
  protected readonly chat       = inject(ChatService);
  private readonly rightPanel = inject(RightPanelService);

  @ViewChild('thread')      private threadEl?: ElementRef<HTMLDivElement>;
  @ViewChild('chunksPanel', { static: true }) private chunksTpl!: TemplateRef<unknown>;

  protected readonly input = signal('');
  private autoScroll = true;

  constructor() {
    effect(() => {
      this.chat.messages();
      this.chat.sending();
      this.autoScroll = true;
    });
  }

  protected canSend(): boolean {
    return this.chat.canSend() && this.input().trim().length > 0;
  }

  protected onInput(value: string) {
    this.input.set(value);
  }

  protected onKeydown(e: KeyboardEvent) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      this.send();
    }
  }

  protected send() {
    if (!this.canSend()) return;
    const q = this.input();
    this.input.set('');
    this.chat.send(q);
  }

  ngAfterViewInit(): void {
    this.rightPanel.set(this.chunksTpl);
  }

  ngOnDestroy(): void {
    this.rightPanel.clear();
  }

  ngAfterViewChecked(): void {
    if (!this.autoScroll || !this.threadEl) return;
    const el = this.threadEl.nativeElement;
    el.scrollTop = el.scrollHeight;
    this.autoScroll = false;
  }
}
