import { Component, EventEmitter, Input, input, Output } from '@angular/core';
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
  @Output() ActiveOption: EventEmitter<'All' | 'Favorites'> = new EventEmitter<
    'All' | 'Favorites'
  >();
  _activeValue: 'All' | 'Favorites' = 'All';
  @Input()
  set activeValue(value: 'All' | 'Favorites') {
    this._activeValue = value;
  }
  onSelectionChange(value: 'All' | 'Favorites') {
    this._activeValue = value;
    this.ActiveOption.emit(this._activeValue);
  }
}
