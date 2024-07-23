import { Component, Output, Input, EventEmitter } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-tag-selector',
  standalone: true,
  imports: [MatButtonToggleModule, MatCheckboxModule],
  templateUrl: './tag-selector.component.html',
  styleUrls: ['./tag-selector.component.scss'],
  template:`
    <div class="tag-selector">
      <button *ngFor="let option of options" [class.selected]="option.value === selectedOption"
              (click)="selectOption(option.value)">
        {{ option.label }}
      </button>
    </div>
  `,
  styles: [`
    .tag-selector {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
    }
    
    button {
      border: 1px solid #ccc;
      border-radius: 4px;
      padding: 4px 12px;
    }
    
    button.selected {
      background-color: #007bff;
      color: white;
    }
  `]
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
