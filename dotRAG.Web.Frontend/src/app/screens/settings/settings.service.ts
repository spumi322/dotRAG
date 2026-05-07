import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { SaveSettingsResult, SettingsDto } from '../../core/api.types';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly http = inject(HttpClient);

  get(): Observable<SettingsDto> {
    return this.http.get<SettingsDto>('/api/settings');
  }

  save(dto: SettingsDto): Observable<SaveSettingsResult> {
    return this.http.put<SaveSettingsResult>('/api/settings', dto);
  }

  reset(): Observable<void> {
    return this.http.delete<void>('/api/settings');
  }
}
