import { Component, computed, inject } from '@angular/core';
import { HealthService } from '../core/health.service';

@Component({
  selector: 'app-status-footer',
  standalone: true,
  templateUrl: './status-footer.component.html',
  styleUrl: './status-footer.component.scss',
})
export class StatusFooterComponent {
  private readonly health = inject(HealthService);

  protected readonly ready = computed(() => this.health.status().ready);
  protected readonly label = computed(() =>
    this.health.status().ready ? 'Ingestion ready' : 'Ingesting…');
  protected readonly meta  = computed(() => {
    const s = this.health.status();
    if (s.chunks === undefined || s.files === undefined) return '— chunks · — files';
    return `${s.chunks} chunks · ${s.files} files`;
  });
}
