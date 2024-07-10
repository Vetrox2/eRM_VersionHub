import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { NgFor } from '@angular/common';

export interface MenuItem {
  icon: string;
  label: string;
  action: string;
}

@Component({
  selector: 'app-menu-icons',
  template: `
    <button
      mat-icon-button
      [matMenuTriggerFor]="menu"
      aria-label="context menu"
    >
      <mat-icon>more_vert</mat-icon>
    </button>
    <mat-menu #menu="matMenu">
      <button
        mat-menu-item
        *ngFor="let item of menuItems"
        (click)="onMenuItemClick(item.action)"
      >
        <mat-icon>{{ item.icon }}</mat-icon>
        <span>{{ item.label }}</span>
      </button>
    </mat-menu>
  `,
  standalone: true,
  imports: [MatButtonModule, MatMenuModule, MatIconModule, NgFor],
})
export class MenuIconsComponent {
  @Input() menuItems: MenuItem[] = [];
  @Output() menuSelection = new EventEmitter<string>();

  onMenuItemClick(action: string) {
    this.menuSelection.emit(action);
  }
}
