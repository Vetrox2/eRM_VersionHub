import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

type ChipSize = 'small' | 'medium' | 'large';

@Component({
  selector: 'app-status-chip',
  standalone: true,
  imports: [CommonModule, MatChipsModule, MatIconModule],
  template: `
    <mat-chip [ngClass]="[statusClass, sizeClass]">
      <span class="chip-content">
        {{ label }}
        <mat-icon [ngClass]="sizeClass">{{ icon }}</mat-icon>
      </span>
    </mat-chip>
  `,
  styles: [
    `
      mat-chip {
        font-weight: bold;
      }
      .chip-content {
        display: flex;
        align-items: center;
        justify-content: center;
      }
      .published {
        background-color: #4caf50 !important;
        color: white;
      }
      .semi-published {
        background-color: #ffc107 !important;
        color: black;
      }
      .not-published {
        background-color: #f44336 !important;
        color: white;
      }
      mat-icon {
        margin-left: 4px;
      }
      .small {
        font-size: 12px;
        padding: 2px 6px;
      }
      .small mat-icon {
        font-size: 14px;
        height: 14px;
        width: 14px;
      }
      .medium {
        font-size: 14px;
        padding: 4px 8px;
      }
      .medium mat-icon {
        font-size: 18px;
        height: 18px;
        width: 18px;
      }
      .large {
        font-size: 16px;
        padding: 6px 12px;
      }
      .large mat-icon {
        font-size: 22px;
        height: 22px;
        width: 22px;
      }
    `,
  ],
})
export class StatusChipComponent {
  @Input() status: 'published' | 'semi-published' | 'not-published' =
    'not-published';
  @Input() size: ChipSize = 'small';

  get statusClass(): string {
    return this.status.replace('-', '');
  }

  get sizeClass(): string {
    return this.size;
  }

  get label(): string {
    return (
      this.status.charAt(0).toUpperCase() +
      this.status.slice(1).replace('-', ' ')
    );
  }

  get icon(): string {
    switch (this.status) {
      case 'published':
        return 'check';
      case 'semi-published':
        return 'remove';
      case 'not-published':
        return 'close';
      default:
        return '';
    }
  }
}
