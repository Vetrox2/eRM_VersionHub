import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-chip-dropdown',
  standalone: true,
  imports: [
    CommonModule,
    MatChipsModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
  ],
  template: `
    <mat-chip (click)="$event.stopPropagation()" [matMenuTriggerFor]="menu">
      <span style="display: flex; align-items: center;">
        {{ selectedOption }}
        <mat-icon style="display: inline;">arrow_drop_down</mat-icon>
      </span>
    </mat-chip>
    <mat-menu #menu="matMenu">
      <mat-selection-list [multiple]="false">
        <mat-list-option
          *ngFor="let option of options"
          (click)="selectOption(option)"
        >
          <div>
            {{ option }}
          </div>
        </mat-list-option>
      </mat-selection-list>
    </mat-menu>
  `,
  styles: [
    `
      mat-chip {
        cursor: pointer;
      }
      mat-menu {
        padding: 0;
      }
      mat-selection-list {
        padding: 0;
      }
      mat-list-option {
        height: 40px;
      }
    `,
  ],
})
export class ChipDropdownComponent {
  @Input() options: string[] = [];
  @Input() selectedOption: string = '';
  @Output() optionSelected = new EventEmitter<string>();

  selectOption(option: string) {
    this.selectedOption = option;
    this.optionSelected.emit(option);
  }
}
