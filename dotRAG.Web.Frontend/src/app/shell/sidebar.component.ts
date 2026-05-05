import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { StatusFooterComponent } from './status-footer.component';

interface NavItem {
  readonly path: string;
  readonly label: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, StatusFooterComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  protected readonly nav: readonly NavItem[] = [
    { path: '/chat',     label: 'Chat' },
    { path: '/notes',    label: 'Notes' },
    { path: '/debug',    label: 'Debug' },
    { path: '/settings', label: 'Settings' },
  ];
}
