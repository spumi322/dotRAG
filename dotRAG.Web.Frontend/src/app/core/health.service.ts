import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { catchError, of, switchMap, timer } from 'rxjs';

export interface HealthStatus {
  readonly ready: boolean;
  readonly chunks?: number;
  readonly files?: number;
}

interface HealthDto {
  status: string;
  checks?: Array<{
    name: string;
    status: string;
    description?: string;
    data?: { chunks?: number; files?: number };
  }>;
}

@Injectable({ providedIn: 'root' })
export class HealthService {
  private static readonly POLL_MS = 5000;
  private readonly http = inject(HttpClient);

  readonly status = signal<HealthStatus>({ ready: false });
  private started = false;

  start() {
    if (this.started) return;
    this.started = true;

    timer(0, HealthService.POLL_MS).pipe(
      switchMap(() => this.http.get<HealthDto>('/health').pipe(
        catchError(() => of<HealthDto>({ status: 'Unhealthy' }))
      ))
    ).subscribe(dto => this.status.set(toStatus(dto)));
  }
}

function toStatus(dto: HealthDto): HealthStatus {
  const ingestion = dto.checks?.find(c => c.name === 'ingestion');
  return {
    ready:  dto.status === 'Healthy',
    chunks: ingestion?.data?.chunks,
    files:  ingestion?.data?.files,
  };
}
