import { Injectable, TemplateRef, signal } from '@angular/core';

// Screens that need a right panel (Chat, Debug) push a TemplateRef on init
// and clear it on destroy. The shell renders a third grid column when set.
@Injectable({ providedIn: 'root' })
export class RightPanelService {
  readonly template = signal<TemplateRef<unknown> | null>(null);

  set(template: TemplateRef<unknown> | null) {
    this.template.set(template);
  }

  clear() {
    this.template.set(null);
  }
}
