import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SettingsDto } from '../../core/api.types';
import { ToastService } from '../../core/toast.service';
import { SettingsService } from './settings.service';

interface FieldDef {
  readonly key: keyof SettingsDto;
  readonly label: string;
  readonly note?: string;
  readonly mono?: boolean;
  readonly type: 'text' | 'number' | 'password';
  readonly section: 'ingestion' | 'chunking' | 'embedding' | 'llm' | 'retrieval';
  readonly reingest?: boolean;
}

const FIELDS: readonly FieldDef[] = [
  // Ingestion
  { key: 'notesPath',        label: 'Notes path',       note: 'relative to content root',         mono: true,  type: 'text',     section: 'ingestion', reingest: true },
  { key: 'fileGlob',         label: 'File glob',        note: 'all subdirectories',                mono: true,  type: 'text',     section: 'ingestion', reingest: true },
  { key: 'minChunkLength',   label: 'Min chunk length', note: 'shorter chunks discarded',                       type: 'number',   section: 'ingestion', reingest: true },

  // Chunking
  { key: 'maxChunkChars',    label: 'Max chunk chars',  note: 'split by paragraph above limit',                 type: 'number',   section: 'chunking',  reingest: true },
  { key: 'headingDepth',     label: 'Heading depth',    note: 'H1..H{depth} treated as breaks',                 type: 'number',   section: 'chunking',  reingest: true },

  // Embedding
  { key: 'embeddingModel',   label: 'Embedding model',  note: 'changing this re-embeds every chunk', mono: true, type: 'text',    section: 'embedding', reingest: true },
  { key: 'voyageApiKey',     label: 'Voyage API key',   note: 'leave masked to keep current',       mono: true,  type: 'password', section: 'embedding' },

  // LLM
  { key: 'llmModel',         label: 'LLM model',        note: 'OpenRouter model id',                mono: true,  type: 'text',     section: 'llm' },
  { key: 'openRouterApiKey', label: 'OpenRouter key',   note: 'leave masked to keep current',       mono: true,  type: 'password', section: 'llm' },

  // Retrieval
  { key: 'topK',             label: 'TopK',             note: 'max chunks per query',                            type: 'number',   section: 'retrieval' },
  { key: 'minScore',         label: 'Min score',        note: '0.0 – 1.0 cosine similarity',                type: 'number',   section: 'retrieval' },
  { key: 'maxPromptTokens',  label: 'Max prompt tokens', note: 'history trimmed to fit',                         type: 'number',   section: 'retrieval' },
];

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export default class SettingsComponent implements OnInit {
  private readonly api    = inject(SettingsService);
  private readonly toast  = inject(ToastService);

  protected readonly loading  = signal(true);
  protected readonly saving   = signal(false);
  protected readonly original = signal<SettingsDto | null>(null);
  protected readonly current  = signal<SettingsDto | null>(null);
  protected readonly reingestPending = signal(false);

  protected readonly dirty = computed(() => {
    const a = this.original();
    const b = this.current();
    if (!a || !b) return false;
    return (Object.keys(a) as (keyof SettingsDto)[]).some(k => a[k] !== b[k]);
  });

  protected readonly leftFields  = FIELDS.filter(f => f.section === 'ingestion' || f.section === 'chunking' || f.section === 'embedding');
  protected readonly rightFields = FIELDS.filter(f => f.section === 'llm'       || f.section === 'retrieval');

  ngOnInit(): void {
    this.load();
  }

  private load() {
    this.loading.set(true);
    this.api.get().subscribe({
      next: dto => {
        this.original.set(structuredClone(dto));
        this.current.set(structuredClone(dto));
        this.loading.set(false);
      },
      error: err => {
        this.toast.error('Failed to load settings');
        this.loading.set(false);
        console.error(err);
      },
    });
  }

  protected setValue(key: keyof SettingsDto, raw: string) {
    const cur = this.current();
    if (!cur) return;
    const def = FIELDS.find(f => f.key === key)!;
    const next = { ...cur };
    if (def.type === 'number') {
      const n = Number(raw);
      (next[key] as unknown as number | null) = Number.isFinite(n) ? n : 0;
    } else {
      (next[key] as unknown as string | null) = raw === '' ? null : raw;
    }
    this.current.set(next);
  }

  protected save() {
    const cur = this.current();
    if (!cur || !this.dirty() || this.saving()) return;

    this.saving.set(true);
    this.api.save(cur).subscribe({
      next: result => {
        this.saving.set(false);
        if (result.reingestTriggered) {
          this.reingestPending.set(true);
          this.toast.info('Settings saved — re-ingesting notes…');
        } else {
          this.toast.success('Settings saved');
        }
        this.load();
      },
      error: err => {
        this.saving.set(false);
        this.toast.error('Save failed');
        console.error(err);
      },
    });
  }

  protected reset() {
    if (this.saving()) return;
    if (!confirm('Reset all settings to defaults? This deletes user-settings.json and re-ingests.')) return;

    this.saving.set(true);
    this.api.reset().subscribe({
      next: () => {
        this.saving.set(false);
        this.reingestPending.set(true);
        this.toast.info('Settings reset — re-ingesting notes…');
        this.load();
      },
      error: err => {
        this.saving.set(false);
        this.toast.error('Reset failed');
        console.error(err);
      },
    });
  }

  protected fieldValue(key: keyof SettingsDto): string {
    const cur = this.current();
    if (!cur) return '';
    const v = cur[key];
    return v === null || v === undefined ? '' : String(v);
  }

  protected sectionTitle(s: FieldDef['section']): string {
    switch (s) {
      case 'ingestion':  return 'Ingestion';
      case 'chunking':   return 'Chunking';
      case 'embedding':  return 'Embedding';
      case 'llm':        return 'LLM';
      case 'retrieval':  return 'Retrieval';
    }
  }

  protected sectionSub(s: FieldDef['section']): string {
    switch (s) {
      case 'ingestion':  return 'Notes source';
      case 'chunking':   return 'MarkdownChunker';
      case 'embedding':  return 'Voyage AI';
      case 'llm':        return 'OpenRouter';
      case 'retrieval':  return 'Vector search tuning';
    }
  }

  protected sectionsFor(fields: readonly FieldDef[]): { id: FieldDef['section']; fields: FieldDef[] }[] {
    const order: FieldDef['section'][] = ['ingestion', 'chunking', 'embedding', 'llm', 'retrieval'];
    return order
      .filter(id => fields.some(f => f.section === id))
      .map(id => ({ id, fields: fields.filter(f => f.section === id) }));
  }
}
