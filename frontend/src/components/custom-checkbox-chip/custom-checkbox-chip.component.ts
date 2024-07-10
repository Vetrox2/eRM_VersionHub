import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-custom-checkbox-chip',
  standalone: true,
  imports: [CommonModule, MatChipsModule, MatIconModule],
  template: `
    <mat-chip-option
      [selected]="checked"
      (selectionChange)="toggleCheck()"
      [class.checked]="checked"
    >
      {{ label }}
    </mat-chip-option>
  `,
  styles: [
    `
      mat-chip-option {
        border: 1px solid #ccc !important;
        background-color: white !important;
      }
      mat-chip-option.checked {
        background-color: #e8f5e9 !important;
        border-color: #4caf50 !important;
        color: #4caf50 !important;
      }
      mat-icon {
        font-size: 18px;
        height: 18px;
        width: 18px;
        margin-right: 4px;
      }
      ::ng-deep .mdc-evolution-chip__action--primary {
        padding-left: 8px !important;
      }
    `,
  ],
})
export class CustomCheckboxChipComponent {
  @Input() label: string = '';
  @Input() checked: boolean = false;
  @Output() checkedChange = new EventEmitter<boolean>();

  toggleCheck() {
    this.checked = !this.checked;
    this.checkedChange.emit(this.checked);
  }
}
