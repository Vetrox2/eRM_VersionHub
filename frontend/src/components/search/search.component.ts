import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search-service.service';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    MatInputModule,
    MatIconModule,
    MatFormFieldModule,
    FormsModule,
    MatButtonModule,
  ],
  template: `
    <mat-form-field class="search-field" appearance="fill">
      @if (showIcons) {
      <mat-icon matIconPrefix>search</mat-icon>
      }
      <input
        matInput
        [placeholder]="placeholder"
        name="search"
        [(ngModel)]="value"
        (ngModelChange)="onInputChange()"
      />
      @if (showIcons && value.length > 0) {
      <button
        matSuffix
        mat-icon-button
        aria-label="Clear"
        (click)="clearSearch()"
      >
        <mat-icon>close</mat-icon>
      </button>
      }
    </mat-form-field>
  `,
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent {
  constructor(private searchService: SearchService) {}
  @Input() showIcons: boolean = true;
  @Input() placeholder: string = 'Search';
  @Output() valueChanged = new EventEmitter<string>();

  value: string = '';

  onInputChange() {
    this.valueChanged.emit(this.value);
  }

  clearSearch() {
    this.value = '';
    this.onInputChange();
  }
}
