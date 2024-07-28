import { Component, EventEmitter, Output } from '@angular/core';
import { signal } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-toggle-app-selector',
  standalone: true,
  imports: [MatCheckboxModule, MatButtonToggleModule],
  templateUrl: './toggle-app-selector.component.html',
  // styleUrl: './toggle-app-selector.component.scss'
})
export class ToggleAppSelectorComponent {
  @Output() ActiveOption: EventEmitter<string> = new EventEmitter<string>();
  activeValue = 'All';

  onSelectionChange(value: string) {
    this.activeValue = value;
    this.ActiveOption.emit(this.activeValue);
  }
}
