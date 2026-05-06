import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { NotesModule, NotesFileSummary } from '../../core/api.types';
import { NotesService } from './notes.service';

interface FilteredModule {
  readonly name: string;
  readonly files: readonly NotesFileSummary[];
}

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.scss',
})
export default class NotesComponent implements OnInit {
  protected readonly notes = inject(NotesService);

  protected readonly searchTerm       = signal('');
  protected readonly expandedModules  = signal<ReadonlySet<string>>(new Set());

  protected readonly filteredModules = computed<FilteredModule[]>(() => {
    const idx = this.notes.index();
    if (!idx) return [];
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) return idx.modules;

    const matches = (s: string) => s.toLowerCase().includes(term);
    const result: FilteredModule[] = [];
    for (const mod of idx.modules) {
      const moduleHit = matches(mod.name);
      const files = mod.files.filter(
        f => moduleHit
          || matches(f.fileName)
          || f.headings.some(matches),
      );
      if (files.length > 0) result.push({ name: mod.name, files });
    }
    return result;
  });

  protected readonly selectedChunk = computed(() => {
    const file = this.notes.selectedFile();
    const idx  = this.notes.selectedChunkIndex();
    if (!file || idx === null) return null;
    return file.chunks[idx] ?? null;
  });

  ngOnInit(): void {
    if (!this.notes.index()) this.notes.loadIndex();
  }

  protected isModuleExpanded(name: string): boolean {
    // Empty search → user-driven expansion. Active search → auto-expand any
    // module that survived the filter so matches are visible.
    if (this.searchTerm().trim()) return true;
    return this.expandedModules().has(name);
  }

  protected toggleModule(name: string) {
    this.expandedModules.update(set => {
      const next = new Set(set);
      if (next.has(name)) next.delete(name);
      else next.add(name);
      return next;
    });
  }

  protected isFileSelected(relativePath: string): boolean {
    return this.notes.selectedFile()?.relativePath === relativePath;
  }

  protected selectFile(file: NotesFileSummary) {
    this.notes.selectFile(file.relativePath);
  }

  protected selectChunk(index: number) {
    this.notes.selectChunk(index);
  }

  protected reindex() {
    this.notes.reindex();
  }

  protected onSearchInput(value: string) {
    this.searchTerm.set(value);
  }

  protected trackByModule(_: number, m: NotesModule | FilteredModule) { return m.name; }
  protected trackByFile(_: number, f: NotesFileSummary) { return f.relativePath; }
}
