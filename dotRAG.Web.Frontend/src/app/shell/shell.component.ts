import { Component, computed, inject } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterOutlet } from '@angular/router';

import { RightPanelService } from '../core/right-panel.service';
import { SidebarComponent } from './sidebar.component';
import { ToastHostComponent } from './toast-host.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, NgTemplateOutlet, SidebarComponent, ToastHostComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
})
export class ShellComponent {
  private readonly rightPanel = inject(RightPanelService);

  protected readonly rightTemplate = this.rightPanel.template;
  protected readonly hasRight = computed(() => this.rightTemplate() !== null);
}
