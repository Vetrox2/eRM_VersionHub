import { Component, signal } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [MatInputModule, MatIconModule, MatFormFieldModule, FormsModule],
  template: `
    <mat-form-field appearance="outline" class="compact-search">
      <mat-label>Search</mat-label>
      <input
        matInput
        type="text"
        [value]="searchTerm()"
        (input)="updateSearch($event)"
        placeholder="Search..."
      />
      <mat-icon matSuffix>search</mat-icon>
    </mat-form-field>
  `,
  styles: [
    `
      .compact-search {
        width: 150px; /* Adjust as needed */
      }
      .compact-search .mat-mdc-text-field-wrapper {
        padding-top: 0;
        padding-bottom: 0;
        height: 36px;
      }
      .compact-search .mat-mdc-form-field-flex {
        min-height: 36px;
        align-items: center;
      }
      .compact-search
        .mat-mdc-text-field-wrapper.mdc-text-field--outlined
        .mat-mdc-form-field-infix {
        padding-top: 4px;
        padding-bottom: 4px;
      }
      .compact-search .mat-mdc-form-field-infix {
        font-size: 14px;
      }
      .compact-search .mat-mdc-form-field-subscript-wrapper {
        display: none;
      }
      .compact-search .mat-icon {
        font-size: 18px;
        width: 14px;
        height: 14px;
      }
      .compact-search .mdc-notched-outline__notch {
        border-right: none;
      }
    `,
  ],
})
export class SearchComponent {
  searchTerm = signal('');

  updateSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }
}
