import { Component, Output, Input, EventEmitter } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-tag-selector',
  standalone: true,
  imports: [MatButtonToggleModule, MatCheckboxModule],
  template: `
    <div class="tag-selector-container">
      <mat-button-toggle-group
        class="tag-selector"
        [value]="selectedOption"
        (change)="selectOption($event.value)"
      >
        @for (option of options; track option.value) {
        <mat-button-toggle [value]="option.value">
          {{ option.label }}
        </mat-button-toggle>
        }
      </mat-button-toggle-group>
    </div>
  `,
  styles: [
    `
      .tag-selector-container {
        width: 100%;
      }
      .tag-selector {
        display: flex;
        flex-wrap: wrap;
        justify-content: flex-start;
      }
      mat-button-toggle {
        flex: 1 1 auto;
        text-align: center;
      }
      @media (max-width: 260px) {
        .tag-selector {
          flex-direction: column;
        }
        mat-button-toggle {
          width: 100%;
          margin-bottom: 8px;
        }
      }
    `,
  ],
})
export class TagSelectorComponent {
  @Output() tagSelected = new EventEmitter<string>();
  @Input() selectedOption: string = 'none';

  options = [
    { value: 'none', label: 'None' },
    { value: 'preview', label: 'Preview' },
    { value: 'scope', label: 'Scope' },
  ];

  selectOption(option: string) {
    this.selectedOption = option;
    this.tagSelected.emit(option);
    console.log(option);
  }
}
